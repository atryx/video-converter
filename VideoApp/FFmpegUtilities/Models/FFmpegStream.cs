using Newtonsoft.Json;

namespace VideoApp.FFmpegUtilities.Models
{
    public class FFmpegStream
    {
        public int Index { get; set; }
        [JsonProperty("codec_name")]
        public string CodecName { get; set; }
        [JsonProperty("codec_long_name")]
        public string CodecLongName { get; set; }
        [JsonProperty("codec_type")]
        public string CodecType { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("sample_aspect_ratio")]
        public string SampleAspectRatio { get; set; }
        [JsonProperty("display_aspect_ratio")]
        public string DisplayAspectRatio { get; set; }
        [JsonProperty("pix_fmt")]
        public string AvailablePxFormat { get; set; }
        [JsonProperty("sample_rate")]
        public long SampleRate { get; set; }
        [JsonProperty("channels")]
        public int Channels { get; set; }
        [JsonProperty("channel_layout")]
        public string ChannelLayout { get; set; }
        [JsonProperty("bit_rate")]
        public long BitRate { get; set; }

    }
}
