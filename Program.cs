using System;
using System.IO;
using CsvHelper;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using kaiten.Services;

namespace kaiten
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var services = new ServiceCollection();
      ConfigureServices(services, args);
      var serviceProvider = services.BuildServiceProvider();
      var logger = serviceProvider.GetService<ILoggerFactory>()
          .CreateLogger<Program>();

      logger.LogDebug("Starting application");
      var config = serviceProvider.GetService<ConfigurationRoot>();
      var kaiten = serviceProvider.GetService<KaitenClient>();
      var jira = serviceProvider.GetService<JiraClient>();
      var fileName = config["input"];
      if (fileName == null)
      {
        logger.LogError("No input file specified");
        return;
      }
      if (!File.Exists(fileName))
      {
        logger.LogError("Input file dosen't exists");
      }
      using var writer = new StreamWriter("foo.csv");
      using var csv = new CsvWriter(writer, new CsvHelper.Configuration.Configuration()
      {
        Delimiter = "|"
      });
      foreach (var line in File.ReadAllLines(fileName))
      {
        var card = await kaiten.GetItem(line);
        if (card.IsEmpty)
        {
          continue;
        }
        var a = await jira.CreateSubTask(card);
      }
    }


    private static void ConfigureServices(IServiceCollection services, string[] args)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("config.json")
          .AddCommandLine(args);
      var config = builder.Build();
      services.AddSingleton(config.GetType(), config);
      services.AddLogging(configure =>
              configure.AddConsole().
              SetMinimumLevel(LogLevel.Debug)
      );
      var kaitenConf = config.GetSection("kaiten");
      services.AddHttpClient("kaiten", client =>
      {
        client.BaseAddress = new Uri(kaitenConf["url"]);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
          config["kaiten:token"]);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
      });
      var jiraConf = config.GetSection("jira");
      var authBytes = System.Text.Encoding.ASCII.GetBytes($"{jiraConf["login"]}:{jiraConf["password"]}");
      services.AddHttpClient("jira", client =>
      {
        client.BaseAddress = new Uri(jiraConf["url"]);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
          "Basic",
          Convert.ToBase64String(authBytes));
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
      });
      services.AddSingleton<KaitenClient>();
      services.AddSingleton<JiraClient>();

    }
  }

}
