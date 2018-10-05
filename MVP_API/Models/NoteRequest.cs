using System.Collections;
using Newtonsoft.Json;

namespace MVP_API
{
    public class NoteRequest
    {
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }
    }
}