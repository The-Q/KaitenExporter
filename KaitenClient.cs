using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kaiten
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

        public async Task<KaitenCardRecord> GetItem(int id)
        {
            var uri = $"cards/{id}";
            _logger.LogDebug("Requesting card {}", id);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var client = _clientFactory.CreateClient("kaiten");
            var response = await client.SendAsync(request);
            _logger.LogDebug($"Got status code {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                var content = await JsonSerializer.DeserializeAsync<KaitenCardRecord>(await response.Content.ReadAsStreamAsync());
                _logger.LogDebug("{}", content);
                content.Link = $"https://crpt.kaiten.io/space/14024/card/{id}";
                return content;
            }
            return KaitenCardRecord.Empty();
        }
    }
}