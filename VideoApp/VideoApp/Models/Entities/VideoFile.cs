using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VideoApp.Web.Models.Entities
{
    public class VideoFile
    {
        [Key]
        public int Id { get; set; }
        public string Filename { get; set; }
        public string FileDirectory { get; set; }
        public TimeSpan Duration { get; set; }
        public long Size { get; set; }
        public string Codec { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public double Framerate { get; set; }
        public string Ratio { get; set; }
        public long BitRate { get; set; }
        public string PixelFormat { get; set; }
        public Status Status { get; set; } = Status.FileUploaded;
        public int? ParentVideoFileId { get; set; }
        public virtual VideoFile ParentVideoFile { get; set; }
        public virtual List<VideoFile> AvailableResolutions { get; set; }
        public virtual List<Thumbnail> Thumbnails { get; set; }
        public virtual List<HLSFile> HLSFiles { get; set; }
    }
}
