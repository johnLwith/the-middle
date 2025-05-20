using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class SegementSubtitleModel
    {
        [JsonPropertyName("segments")]
        public string Segments { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

    }
}
