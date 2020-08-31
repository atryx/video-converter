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

        Task<List<ThumbnailModel>> GetThumbnails(int videoId, ThumbnailDTO thumbnailDTO);
    }
}
