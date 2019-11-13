using kaiten.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace kaiten.Services
{
  class KaitenClient
  {
    private readonly ILogger<KaitenClient> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public KaitenClient(ILoggerFactory loggerFactory, IHttpClientFactory clientFactory)
    {
      _logger = loggerFactory.CreateLogger<KaitenClient>();
      _clientFactory = clientFactory;
    }

    public async Task<KaitenCardRecord> GetItem(string url)
    {
      if(!TryGetCardId(url, out var id))
      {
        _logger.LogWarning($"Unable to get card id for url ${url}");
        return KaitenCardRecord.Empty();
      }
      var uri = $"cards/{id}";
      _logger.LogInformation("Requesting card {}", id);
      var request = new HttpRequestMessage(HttpMethod.Get, uri);
      var client = _clientFactory.CreateClient("kaiten");
      var response = await client.SendAsync(request);
      _logger.LogDebug($"Got status code {response.StatusCode}");
      if (response.IsSuccessStatusCode)
      {
        var content = await JsonSerializer.DeserializeAsync<KaitenCardRecord>(await response.Content.ReadAsStreamAsync());
        _logger.LogDebug("{}", content);
        content.Link = url;
        return content;
      }
      return KaitenCardRecord.Empty();
    }

    private bool TryGetCardId(string url, out int cardId)
    {
      var idPart = url.Split('/', System.StringSplitOptions.RemoveEmptyEntries)
        .ToArr()
        .Last();
      var res = int.TryParse(idPart, out cardId);
      return res;
    }
  }
}