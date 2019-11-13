using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LanguageExt;
using kaiten.Models;

namespace kaiten.Services
{
  class JiraClient
  {
    private readonly ILogger<KaitenClient> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly JiraConfiguration _configuration;

    public JiraClient(ILoggerFactory loggerFactory, IHttpClientFactory clientFactory, ConfigurationRoot configuraton)
    {
      _logger = loggerFactory.CreateLogger<KaitenClient>();
      _clientFactory = clientFactory;
      _configuration = configuraton.GetSection("jira").Get<JiraConfiguration>();
    }

    public async Task<Either<JiraFailureModel, JiraSuccesCreateModel>> CreateSubTask(KaitenCardRecord kaitenCard)
    {
      var uri = "issue/";
      _logger.LogDebug("Creating issue in Jira");
      var request = new HttpRequestMessage(HttpMethod.Post, uri);
      var issue = new JiraIssueModel()
      {
        Assignee = new JiraIssueModel.NamedValue() { Name = _configuration.UserName },
        Description = $"{kaitenCard.Link}\n{kaitenCard.Description}",
        Title = kaitenCard.Title,
        IssueType = JiraIssueModel.IssueTypeModel.GetSubtask(),
        Parent = new JiraIssueModel.ParentModel() { Key = _configuration.ParentId },
        Priority = JiraIssueModel.PriorityModel.GetMedium(),
        Project = new JiraIssueModel.ProjectModel() { Key = _configuration.ProjectName },
        Labels = _configuration.Labels,
        Componets = _configuration.Components.Select(c => new JiraIssueModel.ComponentModel() { Id = c })
      };
      var payload = JsonSerializer.Serialize<JiraIssuePayload>(new JiraIssuePayload() { Issue = issue });
      _logger.LogDebug("Sending this request: {0}",
          JsonSerializer.Serialize<JiraIssueModel>(issue, new JsonSerializerOptions() { WriteIndented = true }));
      var buffer = Encoding.UTF8.GetBytes(payload);
      var byteContent = new ByteArrayContent(buffer);
      byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
      request.Content = byteContent;
      var client = _clientFactory.CreateClient("jira");
      var response = await client.SendAsync(request);
      _logger.LogDebug($"Got status code {response.StatusCode}");
      if (response.IsSuccessStatusCode)
      {
        var streamContent = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<JiraSuccesCreateModel>(streamContent);
        _logger.LogDebug("{}", content);
        return content;
      }
      else
      {
        var streamContent = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<JiraFailureModel>(streamContent);
        _logger.LogDebug("{}", content);
        return content;
      }
    }
  }
}
