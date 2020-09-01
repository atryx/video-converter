using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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

        public async Task<VideoFileModel> ConvertToOtherFormat(FileUploadDTO fileUpload)
        {
            string convertCommandArguments = string.Empty;
            string uniqueFileName = Guid.NewGuid() + "_" + fileUpload.UploadedFile.FileName;
            string fullFilePath = await _fileManagerService.SaveTempFile(fileUpload.UploadedFile, uniqueFileName);
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
            var mediaInfo = await _ffmpeg.GetMediaInfo(fileName);
            var videoFile = _mapper.Map<VideoFile>(mediaInfo);
            videoFile.ParentVideoFileId = parentId;
            videoFile.Filename = fileName;
            videoFile.Status = status;
            _dbContext.Videos.Add(videoFile);
            await _dbContext.SaveChangesAsync();
            return videoFile;
        }       

        public async Task<List<VideoFileModel>> GetAvailableVideos()
        {
            var dbList = await _dbContext.Videos.Where(v => v.ParentVideoFileId == null)
                .Include("DifferentResolutionsFile")
                .Include("Thumbnails")
                .ToListAsync();

            return _mapper.Map<List<VideoFileModel>>(dbList);
        }

        public async Task<VideoFile> GetVideo(int videoId)
        {
            var videoFile = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == videoId);
            return videoFile;
        }     

        public async Task<List<ThumbnailModel>> GetThumbnails(ThumbnailDTO thumbnailDTO)
        {
            var videoFile = await GetVideo(thumbnailDTO.VideoId);
      
            var addedThumbnails = await _ffmpeg.GetVideoThumbails(videoFile.Filename, thumbnailDTO.TimestampOfScreenshots);
            addedThumbnails.ForEach(t => t.ParentVideoFileId = videoFile.Id);
            _dbContext.Thumbnails.AddRange(addedThumbnails);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<List<ThumbnailModel>>(addedThumbnails);

        }

        public async Task GenerateHLS(HLSDTO hlsDTO)
        {
            var videoFile = await GetVideo(hlsDTO.VideoId);

            await _ffmpeg.GenerateHLS(videoFile.Filename, OutputFormat format);
        }
    }
}
