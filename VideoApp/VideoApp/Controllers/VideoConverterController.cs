using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoApp.Web.Models;
using VideoApp.Web.Models.DTOs;
using VideoApp.Web.Models.ViewModels;
using VideoApp.Web.Services;

namespace VideoApp.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideoConverterController : ControllerBase
    {
        private readonly IVideoConverterService _videoConverterService;
        private readonly ILogger<VideoConverterController> _logger;

        public VideoConverterController(ILogger<VideoConverterController> logger, IVideoConverterService videoConverterService)
        {
            _logger = logger;
            _videoConverterService = videoConverterService;
        }

        [HttpPost]
        [RequestSizeLimit(60000000)]
        public async Task<ActionResult<VideoFileModel>> UploadFile([FromForm]FileUploadDTO fileUploadDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid data model");
            }
            var result = await _videoConverterService.ConvertToOtherFormat(fileUploadDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<VideoFileModel>>> GetVideos()
        {
            var result = await _videoConverterService.GetAvailableVideos();
            return Ok(result);
        }

        [HttpGet("{videoId}")]
        public async Task<ActionResult<List<VideoFileModel>>> GetVideo(int videoId)
        {
            var result = await _videoConverterService.GetVideoModel(videoId);
            return Ok(result);
        }

        [HttpPost("thumbnails")]
        public async Task<ActionResult<VideoFileModel>> GenerateThumbnails([FromBody]ThumbnailDTO thumbnailDTO)
        {
            var result = await _videoConverterService.GenerateThumbnails(thumbnailDTO);
            return Ok(result);
        }

        [HttpPost("hls")]
        public async Task<ActionResult<VideoFileModel>> GenerateHLS([FromBody]ConvertVideoDTO hlsDTO)
        {
            var result = await _videoConverterService.GenerateHLS(hlsDTO);
            return Ok(result);
        }

        [HttpPost("convert")]
        public async Task<ActionResult<VideoFileModel>> ConvertFromExistingVideo([FromBody]ConvertVideoDTO hlsDTO)
        {
            var result = await _videoConverterService.ConvertFromExistingVideo(hlsDTO);
            return Ok(result);
        }

        [HttpGet("download/{filename}")]
        public async Task<ActionResult<FileStream>> Download(string filename)
        {
            var file = await _videoConverterService.DownloadFile(filename);
            return Ok(file);
        }
    }
}
