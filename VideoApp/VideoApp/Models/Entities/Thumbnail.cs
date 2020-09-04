using System;
using System.ComponentModel.DataAnnotations;

namespace VideoApp.Web.Models.Entities
{
    public class Thumbnail
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileLocation { get; set; }
        public string Format { get; set; }
        public TimeSpan Timestamp { get; set; }
        public int? ParentVideoFileId { get; set; }
        public virtual VideoFile ParentVideoFile { get; set; }
    }
}
