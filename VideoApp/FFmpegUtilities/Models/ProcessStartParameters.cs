namespace VideoApp.FFmpegUtilities.Models
{
    public class ProcessStartParameters
    {
        public string ConvertedVideoFullPath { get; set; }
        public int ParentVideoFileId { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
    }
}
