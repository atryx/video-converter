using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VideoApp.Web.Database;
using VideoApp.Web.JobQueue;
using VideoApp.Web.Models;
using VideoApp.Web.Models.DTOs;
using VideoApp.Web.Models.Entities;
using VideoApp.Web.Models.ViewModels;
using VideoApp.Web.TaskRunner;
using VideoApp.Web.Utilities;

namespace VideoApp.Web.Services
{
    public class VideoConverterService : IVideoConverterService
    {
        private readonly IJobRunnerQueue _jobRunner;
        private readonly IFFmpegWraperService _ffmpeg;
        private IHostEnvironment _hostingEnvironment;
        private readonly VideoInformationContext _dbContext;
        private readonly IMapper _mapper;

        public VideoConverterService(IHostEnvironment environment,
            VideoInformationContext context,
            IMapper mapper,
            IJobRunnerQueue jobRunner,
            IFFmpegWraperService ffmpeg)
        {
            _hostingEnvironment = environment;
            _dbContext = context;
            _mapper = mapper;
            _jobRunner = jobRunner;
            _ffmpeg = ffmpeg;
        }

        public async Task<VideoFileModel> ConvertToOtherFormat(FileUploadDTO fileUpload)
        {
            string convertCommandArguments = string.Empty;
            string uniqueFileName = Guid.NewGuid() + "_" + fileUpload.UploadedFile.FileName;
            // TODO move this in another services, should return a bool if save is succesfull
            string fullFilePath = await SaveTempFile(fileUpload.UploadedFile, uniqueFileName);
            string outputFile = $"{uniqueFileName.Substring(0, uniqueFileName.LastIndexOf('.'))}_" +
                $"{fileUpload.OutputFormat}." +
                $"{uniqueFileName.Substring(uniqueFileName.LastIndexOf('.'))}";
            var mediaInfo = await  _ffmpeg.GetMediaInfo(uniqueFileName);
            var videoFile = _mapper.Map<VideoFile>(mediaInfo);
            videoFile.Filename = uniqueFileName;
            _dbContext.Videos.Add(videoFile);
            await _dbContext.SaveChangesAsync();

            int id = videoFile.Id;

            _jobRunner.Enqueue(new FFmpegArguments()
            {
                OutputFile = outputFile,
                ParentVideoId = id,
                InputFile = uniqueFileName,
                OutputFormat = fileUpload.OutputFormat
            });
            _jobRunner.JobFinished += c_JobFinished;

            return _mapper.Map<VideoFileModel>(videoFile);
        }

        void c_JobFinished(object sender, CustomEventArgs e)
       {
            try
            {
                UpdateVideoStatus(e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
                SaveVideoFile(e.OutputFile, e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
            
        }

        public async Task UpdateVideoStatus(int id, Status status)
        {
            var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
            video.Status = status;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<VideoFile> SaveVideoFile(string fileName, int parentId = default, Status status = default)
        {
            //var videoInformation = _commandExecuter.GetVideo(fullFilePath);
            var mediaInfo = await _ffmpeg.GetMediaInfo(fileName);
            var videoFile = _mapper.Map<VideoFile>(mediaInfo);
            videoFile.ParentVideoFileId = parentId;
            videoFile.Filename = fileName;
            videoFile.Status = status;
            _dbContext.Videos.Add(videoFile);
            await _dbContext.SaveChangesAsync();
            return videoFile;
        }

        private async Task<string> SaveTempFile(IFormFile file, string fileName)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
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
            _jobRunner.Enqueue(new FFmpegArguments()
            {
                ParentVideoId = videoFile.Id,
                InputFile = videoFile.Filename,
                OutputFile = "something",
                OutputFormat = OutputFormat.Hd720
            });
            _jobRunner.JobFinished += c_JobFinished;

            return new List<ThumbnailModel>();
        }
    }
}
