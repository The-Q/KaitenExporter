using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace kaiten.Models
{
    class JiraFailureModel
    {
        [JsonPropertyName("errorMessages")]
        public IEnumerable<String> Messages { get; set; }
        [JsonPropertyName("errors")]
        public object Errors { get; set; }

    }
}
