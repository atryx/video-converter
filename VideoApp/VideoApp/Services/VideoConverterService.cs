using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                var uniqueDirectory = Guid.NewGuid().ToString();
                _fileManagerService.CreateVideoDirectory(uniqueDirectory);
                var fileLocation = Path.Combine(uniqueDirectory, fileUpload.UploadedFile.FileName);
                var filePath = await _fileManagerService.SaveTempFile(fileUpload.UploadedFile, fileLocation);
                var mediaInfo = await _ffmpeg.GetMediaInfo(filePath);
                var videoFile = _mapper.Map<VideoFile>(mediaInfo);
                videoFile.Filename = fileUpload.UploadedFile.FileName;
                videoFile.FileDirectory = uniqueDirectory;
                await _dbContext.Videos.AddAsync(videoFile);
                await _dbContext.SaveChangesAsync();

                return _mapper.Map<VideoFileModel>(videoFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }       

        public async Task<VideoFile> SaveVideoFile(string fileName, int parentId = default, Status status = default)
        {
            try
            {
                using (var dbContext = new VideoInformationContext())
                {
                    var mediaInfo = await _ffmpeg.GetMediaInfo(fileName);
                    var videoFile = _mapper.Map<VideoFile>(mediaInfo);
                    videoFile.ParentVideoFileId = parentId;
                    videoFile.Filename = Path.GetFileName(fileName);
                    videoFile.FileDirectory = _fileManagerService.GetParentDirectory(fileName);
                    videoFile.Status = status;
                    await dbContext.Videos.AddAsync(videoFile);
                    await dbContext.SaveChangesAsync();
                    return videoFile;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

        public async Task<List<VideoFileModel>> GetAvailableVideos()
        {
            try
            {
                var dbList = await _dbContext.Videos.Where(v => v.ParentVideoFileId == null)
                    .Include("AvailableResolutions")
                    .Include("Thumbnails")
                    .Include("HLSFiles")
                    .ToListAsync();

                return _mapper.Map<List<VideoFileModel>>(dbList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

        

        public async Task<VideoFileModel> GetVideoModel(int id)
        {
            try
            {
                var videoFile = await _dbContext.Videos
                    .Include("Thumbnails")
                    .Include("AvailableResolutions")
                    .Include("HLSFiles")
                    .FirstOrDefaultAsync(v => v.Id == id);
                var result = _mapper.Map<VideoFileModel>(videoFile);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

        public async Task<VideoFileModel> GenerateThumbnails(ThumbnailDTO thumbnailDTO)
        {
            try
            {
                var videoFile = await GetVideo(thumbnailDTO.VideoId);
                string inputFile = Path.Combine(videoFile.FileDirectory, videoFile.Filename);
                var addedThumbnails = await _ffmpeg.GetVideoThumbails(inputFile, thumbnailDTO.TimestampOfScreenshots);
                addedThumbnails.ForEach(t => t.ParentVideoFileId = videoFile.Id);
                await _dbContext.Thumbnails.AddRangeAsync(addedThumbnails);
                await _dbContext.SaveChangesAsync();
                return _mapper.Map<VideoFileModel>(videoFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }

        }

        public async Task<VideoFileModel> GenerateHLS(ConvertVideoDTO hlsDTO)
        {
            try
            {
                var videoFile = await GetVideo(hlsDTO.VideoId);
                await UpdateVideoStatus(videoFile.Id, Status.Processing);

                _jobRunner.Enqueue(new FFmpegArguments()
                {
                    ParentVideoId = videoFile.Id,
                    InputFile = Path.Combine(videoFile.FileDirectory, videoFile.Filename),
                    OutputFormat = hlsDTO.OutputFormat,
                    Operation = OperationType.HLS
                });
                _jobRunner.JobFinished += c_JobFinished;

                return _mapper.Map<VideoFileModel>(videoFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

        public async Task<List<HLSFile>> SaveHLSBatch(string path, OutputFormat format, int parentVideoId)
        {
            try
            {
                using (var dbContext = new VideoInformationContext())
                {
                    var savedFiles = Directory
                            .EnumerateFiles(path)
                            .Where(file => file.Contains(format.ToString()) && (file.ToLower().EndsWith("ts") || file.ToLower().EndsWith("m3u8")))
                            .ToList();


                    foreach (var file in savedFiles)
                    {
                        var extension = file.Substring(file.LastIndexOf('.'));
                        var hls = new HLSFile
                        {
                            Filename = Path.GetFileName(file),
                            FileDirectory = Directory.GetParent(file).Name,
                            ParentVideoFileId = parentVideoId,
                            HLSType = extension.Equals("m3u8") ? HLSType.Playlist : HLSType.PartialVideo
                        };

                        await dbContext.HLS.AddAsync(hls);
                    }

                    await dbContext.SaveChangesAsync();
                    return new List<HLSFile>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

       

        public async Task<FileStream> DownloadFile(string fileName)
        {
            try
            {
                var result = await _fileManagerService.GetFile(fileName);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

        public async Task<VideoFileModel> ConvertFromExistingVideo(ConvertVideoDTO video)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }

        }

        private async Task UpdateVideoStatus(int id, Status status)
        {
            try
            {
                using(var dbContext = new VideoInformationContext())
                {
                    var video = await dbContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
                    video.Status = status;
                    await dbContext.SaveChangesAsync();
                }
              
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }

        }

        private async Task<VideoFile> GetVideo(int videoId)
        {
            try
            {
                var videoFile = await _dbContext.Videos.FirstOrDefaultAsync(v => v.Id == videoId);
                return videoFile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }
        }

        private void c_JobFinished(object sender, CustomEventArgs e)
        {
            try
            {
                UpdateVideoStatus(e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();

                if (e.Operation.Equals(OperationType.Conversion))
                {                    
                    SaveVideoFile(e.OutputFile, e.ParentVideoFileId, Status.DoneProcessing).GetAwaiter();
                }
                else
                {
                    SaveHLSBatch(e.OutputFile, OutputFormat.Hd480, e.ParentVideoFileId).GetAwaiter();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Internal server error");
            }

        }
    }
}
