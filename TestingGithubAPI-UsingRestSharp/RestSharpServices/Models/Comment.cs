using System.Text.Json.Serialization;

namespace RestSharpServices.Models
{
	public class Comment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("body")]
		public string? Body { get; set; }
    }
}
