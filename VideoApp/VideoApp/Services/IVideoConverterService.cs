using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApp.Web.Models;
using VideoApp.Web.Models.DTOs;
using VideoApp.Web.Models.ViewModels;

namespace VideoApp.Web.Services
{
    public interface IVideoConverterService
    {
        Task<VideoFileModel> ConvertToOtherFormat(FileUploadDTO path);

        Task<List<VideoFileModel>> GetAvailableVideos();

        Task<VideoFileModel> GenerateThumbnails(ThumbnailDTO thumbnailDTO);

        Task<VideoFileModel> GenerateHLS(ConvertVideoDTO hlsDTO);

        Task<VideoFileModel> ConvertFromExistingVideo(ConvertVideoDTO video);

        Task<VideoFileModel> GetVideoModel(int id);

        Task<OutputFileModel> DownloadFile(string filename);
    }
}
