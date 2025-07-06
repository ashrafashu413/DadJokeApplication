using System.Text.Json.Serialization;

public class Joke
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("joke")]
    public string JokeText { get; set; }

    [JsonIgnore]
    public JokeLengthGroup Group { get; set; }
}