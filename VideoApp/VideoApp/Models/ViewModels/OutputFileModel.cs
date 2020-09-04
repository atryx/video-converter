using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoApp.Web.Models.ViewModels
{
    public class OutputFileModel
    {
        public byte[] FileContents { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
