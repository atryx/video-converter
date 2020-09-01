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
            var videoSize = _mapper.Map<VideoSize>(format);
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
            var thumbnails = new List<Thumbnail>();
            foreach (var second in wantedSeconds)
            {
                var thumbnail = new Thumbnail();
                thumbnail.Name = $"{inputFile.Substring(0, inputFile.LastIndexOf('.'))}_{second}.png";
                thumbnail.Timestamp = TimeSpan.FromSeconds(second);
                thumbnail.Format = "png";
                thumbnails.Add(thumbnail);
                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(inputPath, thumbnail.Name, TimeSpan.FromSeconds(second));
                IConversionResult result = await conversion.Start();

            }

            return thumbnails;
            
        }

        public async Task GenerateHLS(string inputPath, OutputFormat format)
        {
            var videoSize = _mapper.Map<VideoSize>(format);
            string inputFile = Path.Combine(_basePath, "Uploads", inputPath);
            string hlsDirectory = inputFile.Substring(0, inputFile.LastIndexOf('.'));
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
            IStream videoStream480p = mediaInfo.VideoStreams.FirstOrDefault()
                 ?.SetCodec(VideoCodec.h264)
                 ?.SetSize(videoSize);
            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.aac)
                ?.SetBitrate(128000)
                ?.SetSampleRate(48000);

            IConversionResult result480p = await FFmpeg.Conversions.New()
                    .AddStream(videoStream480p, audioStream)
                    .AddParameter("-profile:v main")
                    .AddParameter("-crf 20")
                    .AddParameter("-g 48 -keyint_min 48")
                    .AddParameter("-sc_threshold 0")
                    .AddParameter("-b:v 2800k - maxrate 2996k - bufsize 4200k ")
                    .AddParameter("-hls_time 4")
                    .AddParameter("-hls_playlist_type vod")
                    .AddParameter($"-hls_segment_filename {hlsDirectory}/{videoSize}_%03d.ts")
                    .AddParameter($"{hlsDirectory}\\{videoSize}.m3u8")
                    .Start();

            // ffmpeg -hide_banner -y -i beach.mkv \
            //-vf scale = w = 640:h = 360:force_original_aspect_ratio = decrease - c:a aac -ar 48000 - c:v h264 -profile:v main -crf 20 - sc_threshold 0 - g 48 - keyint_min 48 - hls_time 4 - hls_playlist_type vod - b:v 800k - maxrate 856k - bufsize 1200k - b:a 96k - hls_segment_filename beach / 360p_ % 03d.ts beach / 360p.m3u8 \
            //-vf scale = w = 842:h = 480:force_original_aspect_ratio = decrease - c:a aac -ar 48000 - c:v h264 -profile:v main -crf 20 - sc_threshold 0 - g 48 - keyint_min 48 - hls_time 4 - hls_playlist_type vod - b:v 1400k - maxrate 1498k - bufsize 2100k - b:a 128k - hls_segment_filename beach / 480p_ % 03d.ts beach / 480p.m3u8 \
            //-vf scale = w = 1280:h = 720:force_original_aspect_ratio = decrease - c:a aac -ar 48000 - c:v h264 -profile:v main -crf 20 - sc_threshold 0 - g 48 - keyint_min 48 - hls_time 4 - hls_playlist_type vod - b:v 2800k - maxrate 2996k - bufsize 4200k - b:a 128k - hls_segment_filename beach / 720p_ % 03d.ts beach / 720p.m3u8 \
            //-vf scale = w = 1920:h = 1080:force_original_aspect_ratio = decrease - c:a aac -ar 48000 - c:v h264 -profile:v main -crf 20 - sc_threshold 0 - g 48 - keyint_min 48 - hls_time 4 - hls_playlist_type vod - b:v 5000k - maxrate 5350k - bufsize 7500k - b:a 192k - hls_segment_filename beach / 1080p_ % 03d.ts beach / 1080p.m3u8
        }

        public async Task<IMediaInfo> GetMediaInfo(string inputPath)
        {
            return await FFmpeg.GetMediaInfo(Path.Combine(_basePath, "Uploads", inputPath));
        }
    }
}
