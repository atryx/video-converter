using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VideoApp.Web.Models.DTOs
{
    public class HLSDTO
    {
        public int VideoId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat OutputFormat { get; set; }
    }
}
