using System.Collections.Generic;
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
    [Route("api/converter")]
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

        [HttpPost("thumbnails")]
        public async Task<ActionResult<List<ThumbnailModel>>> GetThumbnails([FromBody]ThumbnailDTO thumbnailDTO)
        {
            var result = await _videoConverterService.GetThumbnails(thumbnailDTO);
            return Ok(result);
        }
    }
}
