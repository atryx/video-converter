using System.Threading.Tasks;
using VideoApp.Web.Models;

namespace VideoApp.Web.Utilities
{
    public interface IFFmpegWraperService
    {
        Task ConvertToOtherFormat(string inputPath, string outputPath, OutputFormat format);
        Task GetVideoThumbails(string inputPath, string outputPath, int wantedSeconds);
        Task GenerateHLS(string inputPath, string outputPath);
    }
}
