using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace VideoApp.Web.Models
{
    public class FileUploadDTO
    {
        [Required]
        public IFormFile UploadedFile { get; set; }
        [Required]
        public Resolution DesiredResolution { get; set; }        
    }
}
