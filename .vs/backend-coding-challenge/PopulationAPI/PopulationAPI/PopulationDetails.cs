using System.Text.Json.Serialization;

namespace PopulationAPI
{
    public class PopulationDetails
    {
        [JsonPropertyName("ID State")]
        public String  StateID { get; set; }

        [JsonPropertyName("State")]
        public String State { get; set; }

        [JsonPropertyName("ID Year")]
        public int YearID { get; set; }
        [JsonPropertyName("Year")]
        public String Year { get; set; }

        [JsonPropertyName("Population")]
        public int Population { get; set; }

        [JsonPropertyName("Slug State")]
        public string SlugState { get; set; }
    }
}