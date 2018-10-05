using System.Collections.Generic;
using Newtonsoft.Json;

namespace MVP_API
{
    public class NoteResponse
    {
        [JsonProperty("notes")]
        public IEnumerable<Note> Notes { get; set; }
    }
}