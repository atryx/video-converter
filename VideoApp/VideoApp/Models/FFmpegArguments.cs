namespace VideoApp.Web.Models
{
    public class FFmpegArguments
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public OutputFormat OutputFormat { get; set; }
        public int ParentVideoId { get; set; }
        public OperationType Operation { get; set; }
    }

    public enum OperationType
    {
        Conversion,
        HLS
    }
}
