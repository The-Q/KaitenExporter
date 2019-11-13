using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace kaiten.Models
{
    class KaitenCardRecord
    {
        [JsonPropertyName("title")]
        public String Title { get; set; }

        [JsonPropertyName("description")]
        public String Description { get; set; }

        [JsonIgnore]
        public String Link { get; set; }

        public override string ToString()
        {
            return $"{Title} - {Description.Substring(0,20)}...";
        }
        public bool IsEmpty => String.IsNullOrEmpty(Title) && String.IsNullOrEmpty(Description);

        public static KaitenCardRecord Empty()
        {
            return new KaitenCardRecord { Description = String.Empty, Title = String.Empty, Link = String.Empty };
        }
    }
}
