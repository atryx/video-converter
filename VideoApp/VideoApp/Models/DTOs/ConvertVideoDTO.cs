using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VideoApp.Web.Models.DTOs
{
    public class ConvertVideoDTO
    {
        public int VideoId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat OutputFormat { get; set; }
    }
}
