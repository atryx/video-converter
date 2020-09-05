using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IFileManagerService _fileManagerService;
        private readonly VideoInformationContext _dbContext;
        private readonly IMapper _mapper;

        public VideoConverterService(IFileManagerService fileManagerService,
            VideoInformationContext context,
            IMapper mapper,
            IJobRunnerQueue jobRunner,
            IFFmpegWraperService ffmpeg)
        {
            _fileManagerService = fileManagerService;
            _dbContext = context;
            _mapper = mapper;
            _jobRunner = jobRunner;
            _ffmpeg = ffmpeg;
        }

        public async Task<VideoFileModel> UploadFile(FileUploadDTO fileUpload)
        {
            var uniqueDirectory = Guid.NewGuid().ToString();
            _fileManagerService.CreateVideoDirectory(uniqueDirectory);
            var fileLocation = Path.Combine(uniqueDirectory, fileUpload.UploadedFile.FileName);
            var filePath = await _fileManagerService.SaveTempFile(fileUpload.UploadedFile, fileLocation);
            var mediaInfo = await  _ffmpeg.GetMediaInfo(filePath);
            var videoFile = _mapper.Map<VideoFile>(mediaInfo);
            videoFile.Filename = fileUpload.UploadedFile.FileName;
            videoFile.FileDirectory = uniqueDirectory;
            _dbContext.Videos.Add(videoFile);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<VideoFileModel>(videoFile);
        }

       

        public async Task UpdateVideoStatus(int id, Status status)
        {
            var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
            video.Status = status;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<VideoFile> SaveVideoFile(string fileName, int parentId = default, Status status = default)
        {
            try {
                var mediaInfo = await _ffmpeg.GetMediaInfo(fileName);
                var videoFile = _mapper.Map<VideoFile>(mediaInfo);
                videoFile.ParentVideoFileId = parentId;
                videoFile.Filename = Path.GetFileName(fileName);
                videoFile.FileDirectory = _fileManagerService.GetParentDirectory(fileName);
                videoFile.Status = status;
                _dbContext.Videos.Add(videoFile);
                await _dbContext.SaveChangesAsync();
                return videoFile;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }       

        public async Task<List<VideoFileModel>> GetAvailableVideos()
        {
            var dbList = await _dbContext.Videos.Where(v => v.ParentVideoFileId == null)
                .Include("AvailableResolutions")
                .Include("Thumbnails")
                .Include("HLSFiles")
                .ToListAsync();

            return _mapper.Map<List<VideoFileModel>>(dbList);
        }

        private async Task<VideoFile> GetVideo(int videoId)
        {
            var videoFile = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == videoId);
            return videoFile;
        }

        public async Task<VideoFileModel> GetVideoModel(int id)
        {
            var videoFile = await _dbContext.Videos
                .Include("Thumbnails")
                .Include("AvailableResolutions")
                .Include("HLSFiles")
                .FirstOrDefaultAsync(v => v.Id == id);
            var result = _mapper.Map<VideoFileModel>(videoFile);
            return result;
        }

        public async Task<VideoFileModel> GenerateThumbnails(ThumbnailDTO thumbnailDTO)
        {
            var videoFile = await GetVideo(thumbnailDTO.VideoId);
            string inputFile = Path.Combine(videoFile.FileDirectory, videoFile.Filename);
            var addedThumbnails = await _ffmpeg.GetVideoThumbails(inputFile, thumbnailDTO.TimestampOfScreenshots);
            addedThumbnails.ForEach(t => t.ParentVideoFileId = videoFile.Id);
            _dbContext.Thumbnails.AddRange(addedThumbnails);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<VideoFileModel>(videoFile);

        }

        public async Task<VideoFileModel> GenerateHLS(ConvertVideoDTO hlsDTO)
        {
            var videoFile = await GetVideo(hlsDTO.VideoId);

            _jobRunner.Enqueue(new FFmpegArguments()
            {
                ParentVideoId = videoFile.Id,
                InputFile = videoFile.Filename,
                OutputFormat = hlsDTO.OutputFormat,
                Operation = OperationType.HLS
            });
            _jobRunner.JobFinished += c_JobFinished;

            return _mapper.Map<VideoFileModel>(videoFile);
        }

        public async Task<List<HLSFile>> SaveHLSBatch(string path, OutputFormat format, int parentVideoId)
        {

            var savedFiles = Directory.GetFiles(path, $"{format}*");
            var hlsFiles = new List<HLSFile>();
            foreach (var file in savedFiles)
            {
                var extension = file.Substring(file.LastIndexOf('.'));
                var hls = new HLSFile
                {
                    Filename = file,
                    ParentVideoId = parentVideoId,
                    HLSType = extension.Equals("m3u8") ? HLSType.Playlist : HLSType.PartialVideo
                };
                hlsFiles.Add(hls);
            }
            _dbContext.HLS.AddRange(hlsFiles);
            await _dbContext.SaveChangesAsync();
            return hlsFiles;
        }

        void c_JobFinished(object sender, CustomEventArgs e)
        {
            try
            {
                if (e.Operation.Equals(OperationType.Conversion))
                {
                    UpdateVideoStatus(e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
                    SaveVideoFile(e.OutputFile, e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
                }
                else
                {
                    SaveHLSBatch(e.OutputFile,OutputFormat.Hd480, e.ParentVideoFileId).GetAwaiter();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public async Task<FileStream> DownloadFile(string fileName)
        {
            try
            {
                var result = await _fileManagerService.GetFile(fileName);
                return result;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<VideoFileModel> ConvertFromExistingVideo(ConvertVideoDTO video)
        {
            var videoFile = await GetVideo(video.VideoId);
            await UpdateVideoStatus(videoFile.Id, Status.Processing);
            string outFilename = $"{video.OutputFormat.ToString()}{videoFile.Filename.Substring(videoFile.Filename.LastIndexOf('.'))}";
            string outputFile = Path.Combine(videoFile.FileDirectory, outFilename);

            _jobRunner.Enqueue(new FFmpegArguments()
            {
                OutputFile = outputFile,
                ParentVideoId = videoFile.Id,
                InputFile = Path.Combine(videoFile.FileDirectory, videoFile.Filename),
                OutputFormat = video.OutputFormat,
                Operation = OperationType.Conversion
            });
            _jobRunner.JobFinished += c_JobFinished;

            return _mapper.Map<VideoFileModel>(videoFile);
        }
    }
}
