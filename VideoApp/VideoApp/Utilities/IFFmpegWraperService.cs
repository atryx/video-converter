using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApp.Web.Models;
using VideoApp.Web.Models.Entities;
using Xabe.FFmpeg;

namespace VideoApp.Web.Utilities
{
    public interface IFFmpegWraperService
    {
        Task<string> ConvertToOtherFormat(string inputPath, string outputPath, OutputFormat format);
        Task<List<Thumbnail>> GetVideoThumbails(string inputPath, List<int> wantedSeconds);
        Task<string> GenerateHLS(string inputPath, OutputFormat format);
        Task<IMediaInfo> GetMediaInfo(string inputPath);
    }
}
