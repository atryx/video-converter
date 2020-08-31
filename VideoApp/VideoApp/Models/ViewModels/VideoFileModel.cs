using VideoApp.Web.Models.Entities;

namespace VideoApp.Web.Models.ViewModels
{
    public class VideoFileModel
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string FormatName { get; set; }
        public string StartTime { get; set; }
        public string Duration { get; set; }
        public string Size { get; set; }
        public string BitRate { get; set; }
        public string CodecName { get; set; }
        public string Resolution { get; set; }
        public string Status { get; set; }
        public int? ParentVideoFileId { get; set; }
    }
}
