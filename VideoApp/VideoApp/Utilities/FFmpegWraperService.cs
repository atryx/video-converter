using AutoMapper;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoApp.Web.Models;
using VideoApp.Web.Models.Entities;
using Xabe.FFmpeg;

namespace VideoApp.Web.Utilities
{
    public class FFmpegWraperService : IFFmpegWraperService
    {
        private IHostEnvironment _hostingEnvironment;
        private IMapper _mapper;
        private string _basePath;

        public FFmpegWraperService(IHostEnvironment hostingEnvironment, IMapper mapper)
        {
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _basePath = _hostingEnvironment.ContentRootPath;

            if (string.IsNullOrEmpty(FFmpeg.ExecutablesPath))
            {
                FFmpeg.SetExecutablesPath(Path.Combine(_basePath, "ffmpeg\\bin"));
            }
        }

        public async Task ConvertToOtherFormat(string inputFile, string outputFile, OutputFormat format)
        {
            var videoSize = ConvertEnum(format);
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(Path.Combine(_basePath, "Uploads", inputFile));
            string output = Path.Combine(_basePath, "Uploads", outputFile);

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.SetSize(videoSize);

            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.aac);


            await FFmpeg.Conversions.New()
                .AddStream(audioStream, videoStream)
                .SetOutput(output)
                .Start();
        }

        public async Task<List<Thumbnail>> GetVideoThumbails(string inputFile, List<int> wantedSeconds)
        {
            string inputPath = Path.Combine(_basePath, "Uploads", inputFile);
            string basePath = Path.Combine(_basePath, "Uploads");
            string fileDirectory = inputPath.Substring(0, inputPath.LastIndexOf('.'));
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            var thumbnails = new List<Thumbnail>();
            foreach (var second in wantedSeconds)
            {
                var thumbnail = new Thumbnail();
                thumbnail.Name = $"Thumnbail_{second}.png";
                thumbnail.FileLocation = $"{inputFile.Substring(0, inputFile.LastIndexOf('.'))}\\{thumbnail.Name}";
                thumbnail.Timestamp = TimeSpan.FromSeconds(second);
                thumbnail.Format = "png";
                thumbnails.Add(thumbnail);
                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(inputPath, 
                    $"{basePath}\\{thumbnail.FileLocation}", 
                    TimeSpan.FromSeconds(second));
                IConversionResult result = await conversion.Start();

            }

            return thumbnails;
            
        }

        public async Task<string> GenerateHLS(string inputPath, OutputFormat format)
        {
            string convertParams = "-profile:v main -crf 20 -sc_threshold 0 -g 48 -keyint_min 48 -hls_time 4 -hls_playlist_type vod ";
            var videoSize = ConvertEnum(format);
            string inputFile = Path.Combine(_basePath, "Uploads", inputPath);
            string fileDirectory = inputFile.Substring(0, inputFile.LastIndexOf('.'));
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                 ?.SetCodec(VideoCodec.h264)
                 ?.SetSize(videoSize);
            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.aac)
                ?.SetBitrate(128000)
                ?.SetSampleRate(48000);

            switch (videoSize)
            {                
                case VideoSize.Hd480:
                    convertParams += $"-b:v 1400k -maxrate 1498k -bufsize 2100k -b:a 128k -hls_segment_filename {fileDirectory}\\480p_%03d.ts {fileDirectory}\\480p.m3u8";
                    break;
                case VideoSize.Hd720:
                    convertParams += $"-b:v 2800k -maxrate 2996k -bufsize 4200k -b:a 128k -hls_segment_filename {fileDirectory}\\720p_%03d.ts {fileDirectory}\\720p.m3u8";
                    break;
                case VideoSize.Hd1080:
                    convertParams += $"-b:v 5000k -maxrate 5350k -bufsize 7500k -b:a 192k -hls_segment_filename {fileDirectory}\\1080p_%03d.ts {fileDirectory}\\1080p.m3u8";
                    break;                
                default:
                    convertParams += $"-b:v 800k -maxrate 856k -bufsize 1200k -b:a 96k -hls_segment_filename {fileDirectory}\\default_%03d.ts {fileDirectory}\\default.m3u8";
                    break;
            }

            IConversionResult result = await FFmpeg.Conversions.New()
                    .AddStream(videoStream, audioStream)
                    .AddParameter(convertParams)
                    .Start();

            return fileDirectory;
        }

        public async Task<IMediaInfo> GetMediaInfo(string inputPath)
        {
            return await FFmpeg.GetMediaInfo(Path.Combine(_basePath, "Uploads", inputPath));
        }

        private VideoSize ConvertEnum(OutputFormat format)
        {
            VideoSize videoSize;
            switch (format)
            {
                case OutputFormat.Hd480:
                    videoSize = VideoSize.Hd480;
                    break;
                case OutputFormat.Hd720:
                    videoSize = VideoSize.Hd720;
                    break;
                case OutputFormat.Hd1080:
                    videoSize = VideoSize.Hd1080;
                    break;
                default:
                    videoSize = VideoSize.Hd720;
                    break;
            }
            return videoSize;
        }
    }
}
