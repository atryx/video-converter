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
        private string _basePath;

        public FFmpegWraperService(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _basePath = _hostingEnvironment.ContentRootPath;

            if (string.IsNullOrEmpty(FFmpeg.ExecutablesPath))
            {
                FFmpeg.SetExecutablesPath(Path.Combine(_basePath, "ffmpeg\\bin"));
            }
        }

        public async Task<string> ConvertToOtherFormat(string inputFile, string outputFile, OutputFormat format)
        {
            var videoSize = ConvertEnum(format);
            string inputPath = Path.Combine(_basePath, "Uploads", inputFile);
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(Path.Combine(_basePath, "Uploads", inputPath));
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

            return output;
        }

        public async Task<List<Thumbnail>> GetVideoThumbails(string inputFile, List<int> wantedSeconds)
        {
            string inputPath = Path.Combine(_basePath, "Uploads", inputFile);
            string basePath = Path.Combine(_basePath, "Uploads");
            string parentDirectory = Directory.GetParent(inputPath).Name;

            var thumbnails = new List<Thumbnail>();
            foreach (var second in wantedSeconds)
            {
                var thumbnail = new Thumbnail();
                thumbnail.Name = $"Thumnbail_{second}.png";
                thumbnail.FileDirectory = parentDirectory;
                thumbnail.Timestamp = TimeSpan.FromSeconds(second);
                thumbnail.Format = "png";
                thumbnails.Add(thumbnail);
                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(inputPath,
                    $"{basePath}\\{thumbnail.FileDirectory}\\{thumbnail.Name}",
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

            string fileDirectory = Directory.GetParent(inputFile).Name;
            string fullDirectory = Path.Combine(_basePath, "Uploads", fileDirectory);

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
                    convertParams += $"-b:v 1400k -maxrate 1498k -bufsize 2100k -b:a 128k -hls_segment_filename {fullDirectory}\\{format}_%03d.ts {fullDirectory}\\{format}.m3u8";
                    break;
                case VideoSize.Hd720:
                    convertParams += $"-b:v 2800k -maxrate 2996k -bufsize 4200k -b:a 128k -hls_segment_filename {fullDirectory}\\{format}_%03d.ts {fullDirectory}\\{format}.m3u8";
                    break;
                case VideoSize.Hd1080:
                    convertParams += $"-b:v 5000k -maxrate 5350k -bufsize 7500k -b:a 192k -hls_segment_filename {fullDirectory}\\{format}_%03d.ts {fullDirectory}\\{format}.m3u8";
                    break;
                default:
                    break;
            }

            IConversionResult result = await FFmpeg.Conversions.New()
                    .AddStream(videoStream, audioStream)
                    .AddParameter(convertParams)
                    .Start();

            return fullDirectory;
        }

        public async Task<IMediaInfo> GetMediaInfo(string inputPath)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(inputPath);
            return mediaInfo;

        }

        private VideoSize ConvertEnum(OutputFormat format)
        {
            var videoSize = format switch
            {
                OutputFormat.Hd480 => VideoSize.Hd480,
                OutputFormat.Hd720 => VideoSize.Hd720,
                OutputFormat.Hd1080 => VideoSize.Hd1080,
                _ => VideoSize.Hd720,
            };
            return videoSize;
        }
    }
}
