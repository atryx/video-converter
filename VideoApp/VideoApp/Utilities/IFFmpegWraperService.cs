using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApp.Web.Models;
using VideoApp.Web.Models.ViewModels;
using Xabe.FFmpeg;

namespace VideoApp.Web.Utilities
{
    public interface IFFmpegWraperService
    {
        Task ConvertToOtherFormat(string inputPath, string outputPath, OutputFormat format);
        Task<List<ThumbnailModel>> GetVideoThumbails(string inputPath, List<int> wantedSeconds);
        Task GenerateHLS(string inputPath, string outputPath);
        Task<IMediaInfo> GetMediaInfo(string inputPath);
    }
}
