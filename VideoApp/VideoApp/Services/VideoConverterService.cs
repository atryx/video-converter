using AutoMapper;
using FFmpegUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VideoApp.FFmpegUtilities.Models;
using VideoApp.Web.Database;
using VideoApp.Web.Models;
using VideoApp.Web.Models.DTOs;
using VideoApp.Web.Models.Entities;
using VideoApp.Web.Models.ViewModels;
using VideoApp.Web.TaskRunner;

namespace VideoApp.Web.Services
{
    public class VideoConverterService : IVideoConverterService
    {
        private readonly JobRunnerQueue _jobRunner = new JobRunnerQueue();
        private readonly CommandExecuter _commandExecuter = new CommandExecuter();
        private IHostEnvironment _hostingEnvironment;
        private readonly VideoInformationContext _dbContext;
        private readonly IMapper _mapper;

        public VideoConverterService(IHostEnvironment environment,
            VideoInformationContext context,
            IMapper mapper)
        {
            _hostingEnvironment = environment;
            _dbContext = context;
            _mapper = mapper;

        }

        public async Task<VideoFileModel> ConvertToOtherFormat(FileUploadDTO fileUpload)
        {
            string convertCommandArguments = string.Empty;
            string uniqueFileName = Guid.NewGuid() + "_" + fileUpload.UploadedFile.FileName;
            string fullFilePath = await SaveTempFile(fileUpload.UploadedFile, uniqueFileName);
            string convertedFileName = $"{fullFilePath.Substring(0, fullFilePath.LastIndexOf('.'))}_{fileUpload.DesiredResolution.Width}x{fileUpload.DesiredResolution.Height}.mkv";
            var videoInformation = _commandExecuter.GetVideo(fullFilePath);
            foreach (var stream in videoInformation.Streams)
            {
                if (stream.CodecType.Equals("video") && stream.CodecName.Equals("h264"))
                {
                    convertCommandArguments = $"-i {fullFilePath} -vf scale={fileUpload.DesiredResolution.Width}:{fileUpload.DesiredResolution.Height}" +
                        $" -crf 18 -preset slow -c:a copy {convertedFileName}";
                }
                else
                {
                    convertCommandArguments = $"-i {fullFilePath} -vf scale={fileUpload.DesiredResolution.Width}:{fileUpload.DesiredResolution.Height}" +
                                            $" -c:v libx264 -crf 18 -preset slow -c:a copy {convertedFileName}";
                }
                break;
            }
            var videoFile = _mapper.Map<VideoFile>(videoInformation);
            _dbContext.Videos.Add(videoFile);
            await _dbContext.SaveChangesAsync();
            int id = videoFile.Id;

            _jobRunner.Enqueue(new ProcessStartParameters()
            {
                ConvertedVideoFullPath = convertedFileName,
                ParentVideoFileId = id,
                Command = "ffmpeg",
                Arguments = convertCommandArguments
            });
            _jobRunner.JobFinished += c_JobFinished;

            return _mapper.Map<VideoFileModel>(videoFile);
        }

        void c_JobFinished(object sender, CustomEventArgs e)
       {
            Console.WriteLine("FFMPEG finished executing the process");
            UpdateVideoStatus(e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
            SaveVideoFile(e.ConvertedFileFullpath, e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
        }

        public async Task UpdateVideoStatus(int id, Status status)
        {
            var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
            video.Status = status;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<VideoFile> SaveVideoFile(string fullFilePath, int parentId = default, Status status = default)
        {
            var videoInformation = _commandExecuter.GetVideo(fullFilePath);
            var videoFile = _mapper.Map<VideoFile>(videoInformation);
            videoFile.ParentVideoFileId = parentId;
            videoFile.Status = status;
            _dbContext.Videos.Add(videoFile);
            await _dbContext.SaveChangesAsync();
            return videoFile;
        }

        private async Task<string> SaveTempFile(IFormFile file, string fileName)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "TempLocation");
            if (file.Length > 0)
            {
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return filePath;
            }
            else
            {
                throw new Exception("file must not be empty");
            }
        }

        public async Task<List<VideoFileModel>> GetAvailableVideos()
        {
            var dbList = await _dbContext.Videos.ToListAsync();
            return _mapper.Map<List<VideoFileModel>>(dbList);
        }

        public async Task<VideoFile> GetVideo(int videoId)
        {
            var videoFile = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == videoId);
            return videoFile;
        }

        private async Task<bool> DeleteTempFile(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return await Task.FromResult(true);
                }
                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleteing TEMP file: " + ex.Message);
            }
        }

        public async Task<List<ThumbnailModel>> GetThumbnails(int videoId, ThumbnailDTO thumbnailDTO)
        {
            var videoFile = await GetVideo(videoId);
            string convertCommandArguments = $"-i {videoFile.Filename}";
            string fileNameWOExtension = videoFile.Filename.Substring(0, videoFile.Filename.LastIndexOf('.'));
            int i = 1;
            foreach (var timestamp in thumbnailDTO.TimestampOfScreenshots)
            {
                convertCommandArguments += $"-ss {timestamp} -vframes 1 {fileNameWOExtension}_{i}.png";
                i++;
            }
            _jobRunner.Enqueue(new ProcessStartParameters()
            {
                ParentVideoFileId = videoFile.Id,
                Command = "ffmpeg",
                Arguments = convertCommandArguments
            });
            _jobRunner.JobFinished += c_JobFinished;

            return new List<ThumbnailModel>();
        }
    }
}
