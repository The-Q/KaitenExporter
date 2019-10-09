using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Kaiten.Models
{

    class JiraIssuePayload
    {
        [JsonPropertyName("fields")]
        public JiraIssueModel Issue { get; set; }
    }
    class JiraIssueModel
    {
        [JsonPropertyName("project")]
        public ProjectModel Project { get; set; }

        [JsonPropertyName("parent")]
        public ParentModel Parent { get; set; }

        [JsonPropertyName("issuetype")]
        public IssueTypeModel IssueType { get; set; }

        [JsonPropertyName("summary")]
        public String Title { get; set; }

        [JsonPropertyName("description")]
        public String Description { get; set; }

        [JsonPropertyName("assignee")]
        public NamedValue Assignee { get; set; }

        [JsonPropertyName("priority")]
        public PriorityModel Priority { get; set; }

        internal interface IKeyedValue
        {
            public String Key { get; set; }
        }
        internal class ProjectModel : IKeyedValue
        {
            [JsonPropertyName("key")]
            public String Key { get; set; }
        }

        internal class ParentModel : IKeyedValue
        {
            [JsonPropertyName("key")]
            public String Key { get; set; }
        }
        internal interface IDValue
        {
            public String Id { get; set; }
        }

        internal class IssueTypeModel : IDValue
        {
            [JsonPropertyName("id")]
            public String Id { get; set; }
            public static IssueTypeModel GetSubtask()
            {
                return new IssueTypeModel() { Id = "10003" };
            }
        }

        internal class PriorityModel : IDValue
        {
            [JsonPropertyName("id")]
            public String Id { get; set; }

            public static PriorityModel GetCritical()
            {
                return new PriorityModel() { Id = "1" };
            }
            public static PriorityModel GetHigh()
            {
                return new PriorityModel() { Id = "2" };
            }
            public static PriorityModel GetMedium()
            {
                return new PriorityModel() { Id = "3" };
            }
            public static PriorityModel GetLow()
            {
                return new PriorityModel() { Id = "5" };
            }
        }

        internal class NamedValue
        {
            [JsonPropertyName("name")]
            public String Name { get; set; }
        }
        
    }
}
