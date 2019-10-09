using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace kaiten
{
    class JiraSuccesCreateModel
    {
        [JsonPropertyName("id")]
        public String Id { get; set; }
        [JsonPropertyName("key")]
        public String Key { get; set; }
        [JsonPropertyName("self")]
        public String SelfURI { get; set; }
    }
}
