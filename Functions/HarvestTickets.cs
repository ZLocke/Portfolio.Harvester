using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Portfolio.ETL;

public class HarvestTickets
{
    private readonly TelemetryClient _logger;
    private readonly ZendeskClient _zendeskClient;
    private readonly TicketTransformer _ticketTransformer;
    private readonly SqlClient _sqlClient;

    private DateTime _since = default;

    public HarvestTickets(TelemetryConfiguration telemetryConfiguration, ZendeskClient zendeskClient, TicketTransformer ticketTransformer, SqlClient sqlClient)
    {
        _logger = new TelemetryClient(telemetryConfiguration);
        _zendeskClient = zendeskClient;
        _ticketTransformer = ticketTransformer;
        _sqlClient = sqlClient;
    }

    private const string _FunctionName = "HarvestTickets";
    [FunctionName(_FunctionName)]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "{ago}")] HttpRequest req, int ago = 0)
    {
        _logger.TrackEvent($"{_FunctionName} called.");
        if (ago <= 0) return new BadRequestObjectResult("The ago parameter must be a positive integer.");

        try
        {
            _since = DateTime.UtcNow.AddHours(-ago);
            string exportQuery = GetExportQuery();
            await Harvest(exportQuery);
            return new OkObjectResult("Finished");
        }
        catch (Exception ex)
        {
            _logger.TrackException(new ExceptionTelemetry(ex));
            _logger.TrackException(ex, new Dictionary<string, string> { { "Message", "This is just the exception." } });
            return new BadRequestObjectResult("Failed to harvest tickets.");
        }
    }

    // We convert the lookback value to a specific format that Zendesk can handle. 
    // More info on formatting datetimes here: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
    // You can find info on this query syntax here: https://developer.zendesk.com/api-reference/ticketing/ticket-management/search/#query-basics
    private string GetExportQuery()
    {
        string lookback = _since.ToString("d");
        return $"search/export?filter[type]=ticket&query=updated>{lookback}";
    }

    private async Task Harvest(string query)
    {
        try
        {
            // Request the tickets from Zendesk API.
            ZendeskExtraction extraction = await Extract(query);
            if (extraction.Tickets.Count == 0) return;

            // Tranform the tickets from the Zendesk Model to the Sql Model
            Transform(extraction, out List<SqlTicketModel> newTickets, out List<SqlTicketModel> updatedTickets);

            // Apply changes to the database.
            Load(newTickets, updatedTickets);

            Dictionary<string, string> properties = new Dictionary<string, string>()
            {
                {"TicketsSince", _since.ToString("d")}
            };

            Dictionary<string, double> metrics = new Dictionary<string, double>()
            {
                {"TicketsUpdated", updatedTickets.Count },
                {"TicketsInserted", newTickets.Count }
            };

            _logger.TrackEvent($"{_FunctionName} completed.", properties, metrics);
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }
    }

    private async Task<ZendeskExtraction> Extract(string query)
    {
        try
        {
            // Call the Zendesk API
            List<ZendeskTicketModel> tickets = await _zendeskClient.ExtractTickets(query);

            // Remove any tickets that are already up to date.
            List<long> ids = _sqlClient.GetUpToDateTickets(_since);
            tickets.RemoveAll(t => ids.Contains(t.id));

            // Call Zendesk API to get sidepulls, which are used to join with the ticket models in transform stage.
            Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>> sidepulls = await _zendeskClient.ExtractSidepulls();

            if (tickets.Count == 0) _logger.TrackTrace("All tickets in timerange are up to date.");
            return new ZendeskExtraction(tickets, sidepulls);
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }
    }

    private void Transform(ZendeskExtraction extraction, out List<SqlTicketModel> newTickets, out List<SqlTicketModel> updatedTickets)
    {
        try
        {
            _logger.TrackTrace("Transforming harvested tickets");
            newTickets = _ticketTransformer.Transform(extraction);

            _logger.TrackTrace("Separating new and stale tickets");
            List<long> staleTicketIds = _sqlClient.GetStaleTickets(_since, newTickets);
            updatedTickets = newTickets.Where(t => staleTicketIds.Contains(t.Id)).ToList()
                                                    ?? new List<SqlTicketModel>();
            for (int i = 0; i < updatedTickets.Count; i++) newTickets.Remove(updatedTickets[i]);
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }
    }

    private void Load(List<SqlTicketModel> newTickets, List<SqlTicketModel> updatedTickets)
    {
        try
        {
            _sqlClient.UpdateTickets(updatedTickets);
            _sqlClient.InsertTickets(newTickets);
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }
    }
}

