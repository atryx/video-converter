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
                OutputFormat = fileUpload.OutputFormat,
                Operation = OperationType.Conversion
            });
            _jobRunner.JobFinished += c_JobFinished;

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
            var videoFile = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
            var result = _mapper.Map<VideoFileModel>(videoFile);
            return result;
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

            _jobRunner.Enqueue(new FFmpegArguments()
            {
                ParentVideoId = videoFile.Id,
                InputFile = videoFile.Filename,
                OutputFormat = hlsDTO.OutputFormat,
                Operation = OperationType.HLS
            });
            _jobRunner.JobFinished += c_JobFinished;            
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
                    FileName = file,
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
    }
}
