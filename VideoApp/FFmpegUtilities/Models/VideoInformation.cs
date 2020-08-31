using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VideoApp.FFmpegUtilities.Models
{
    public class VideoInformation
    {
        public List<FFmpegStream> Streams{ get; set; }
        public Format Format { get; set; }
    }
}
