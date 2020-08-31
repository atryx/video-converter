using System;

namespace VideoApp.Web.Models.ViewModels
{
    public class ThumbnailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Format { get; set; }
        public TimeSpan Timestamp { get; set; }
    }
}
