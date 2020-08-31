using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VideoApp.Web.Models.Entities
{
    public class VideoFile
    {
        [Key]
        public int Id { get; set; }
        public string Filename { get; set; }
        public string FormatName { get; set; }
        public string StartTime { get; set; }
        public string Duration { get; set; }
        public string Size { get; set; }
        public string BitRate { get; set; }
        public string CodecName { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string SampleAspectRatio { get; set; }
        public string DisplayAspectRatio { get; set; }
        public Status Status { get; set; } = Status.FileUploaded;
        public int? ParentVideoFileId { get; set; }
        public virtual VideoFile ParentVideoFile { get; set; }
        public virtual List<VideoFile> DifferentResolutionsFile { get; set; } 
    }
}
