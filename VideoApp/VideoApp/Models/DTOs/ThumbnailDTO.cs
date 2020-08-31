using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoApp.Web.Models.DTOs
{
    public class ThumbnailDTO
    {
        public int VideoId { get; set; }
        public List<int> TimestampOfScreenshots { get; set; }
    }
}
