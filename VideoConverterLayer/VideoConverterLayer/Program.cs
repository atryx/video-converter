using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace VideoConverterLayer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FFmpeg.SetExecutablesPath("C:\\Users\\mihai.lita\\Downloads\\ffmpeg-20200828-ccc7120-win64-static\\bin");
            string filePath = Path.Combine("C:\\Users\\mihai.lita\\Desktop", "london_riverThames_eye_ben.mp4");
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Console.WriteLine(path);

            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filePath);

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.Reverse()
                ?.SetSize(VideoSize.Hd720);

            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.aac);


            await FFmpeg.Conversions.New()
                .AddStream(audioStream, videoStream)
                .SetOutput(outputPath)
                .Start();

            string output = Path.ChangeExtension(Path.GetTempFileName(), ".mkv");
            string input = Path.Combine("C:", "Users\\mihai.lita\\Desktop", "london_riverThames_eye_ben.mp4"); ;

            var conversion = await FFmpeg.Conversions.FromSnippet.ChangeSize(input, output, VideoSize.Hd720);

            IConversionResult result = await conversion.Start();
        }
    }
}
