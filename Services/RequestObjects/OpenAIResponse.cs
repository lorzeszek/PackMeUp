using System.Text.Json.Serialization;

namespace PackMeUp.Services.RequestObjects
{
    public class OpenAIResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        public class Choice
        {
            [JsonPropertyName("message")]
            public Message Message { get; set; }
        }

        public class Message
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    }
}
