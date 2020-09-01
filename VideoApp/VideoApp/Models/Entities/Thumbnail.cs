using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoApp.Web.Models.Entities
{
    public class Thumbnail
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public TimeSpan Timestamp { get; set; }
        public int? ParentVideoFileId { get; set; }
        public virtual VideoFile ParentVideoFile { get; set; }
    }
}
