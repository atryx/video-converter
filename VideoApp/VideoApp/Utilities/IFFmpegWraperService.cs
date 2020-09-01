using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApp.Web.Models;
using VideoApp.Web.Models.Entities;
using Xabe.FFmpeg;

namespace VideoApp.Web.Utilities
{
    public interface IFFmpegWraperService
    {
        Task ConvertToOtherFormat(string inputPath, string outputPath, OutputFormat format);
        Task<List<Thumbnail>> GetVideoThumbails(string inputPath, List<int> wantedSeconds);
        Task GenerateHLS(string inputPath);
        Task<IMediaInfo> GetMediaInfo(string inputPath);
    }
}
