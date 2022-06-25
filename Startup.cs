using System.IO;
using System.Net.Http;
using System.Text;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Portfolio.ETL.Startup))]
namespace Portfolio.ETL;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        IConfiguration config = BuildConfig();
        builder.Services.AddSingleton(config);
        AddTelemetryConfig(builder);
        AddZendeskHttpClient(builder);
        AddETLComponents(builder);
    }

    private static IConfiguration BuildConfig()
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //  .AddJsonFile("secret.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        return configBuilder.Build();
    }

    private static void AddTelemetryConfig(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton(sp =>
        {
            TelemetryConfiguration telemetryConfiguration = new()
            {
                InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")
            };
            telemetryConfiguration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            return telemetryConfiguration;
        });
    }

    private static void AddZendeskHttpClient(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient("Zendesk", client =>
        {
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BASE_ADDRESS"));
            string encodedHeader = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                     .GetBytes(Environment.GetEnvironmentVariable("AUTH")));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {encodedHeader}");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });
    }

    private static void AddETLComponents(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<ZendeskClient>();
        builder.Services.AddSingleton<TicketTransformer>();
        builder.Services.AddSingleton<SqlClient>();
        builder.Services.AddSingleton<SQLExecutionService>();
    }
}
