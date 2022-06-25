namespace Portfolio.ETL;

/// <summary>
/// Makes http calls to Zendesk API
/// </summary>
public class ZendeskClient
{
    private readonly TelemetryClient _logger;
    private readonly HttpClient _httpClient;

    public ZendeskClient(TelemetryConfiguration telemetryConfiguration, IHttpClientFactory factory)
    {
        _logger = new TelemetryClient(telemetryConfiguration);
        _httpClient = factory.CreateClient("Zendesk");
    }

    public async Task<List<ZendeskTicketModel>> ExtractTickets(string exportQuery)
    {
        _logger.TrackTrace("Requesting Tickets from Zendesk");
        List<ZendeskTicketModel> tickets = await Export(exportQuery);
        return tickets;
    }

    private async Task<T> RequestAs<T>(string query)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<T>();
    }

    private async Task<List<ZendeskTicketModel>> Export(string exportQuery)
    {
        try
        {
            List<ZendeskTicketModel> allTickets = new List<ZendeskTicketModel>();
            string route = exportQuery;
            while (true)
            {
                ExportSearchResults searchResults = await RequestAs<ExportSearchResults>(route);
                if (searchResults is null) throw new ArgumentNullException();
                ZendeskTicketModel[] tickets = searchResults.Results ?? Array.Empty<ZendeskTicketModel>();
                if (tickets == null || tickets.Length == 0) break;
                allTickets.AddRange(tickets);
                string newRoute = searchResults.Links?.Next ?? string.Empty;
                if (string.IsNullOrEmpty(newRoute)) break;
                newRoute = newRoute.Replace(_httpClient.BaseAddress!.ToString(), "");
                route = newRoute;
            }
            return allTickets;
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }

    }

    /// <summary>
    /// Makes HTTP requests to Zendesk Search endpoint to get the users, groups, and other entities from Zendesk.
    /// These are joined with the tickets to replace id references with the actual names.
    /// </summary>
    /// <returns>Dictionary of results by model type</returns>
    public async Task<Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>>> ExtractSidepulls()
    {
        _logger.TrackTrace("Requesting sidepulls to join with tickets.");
        Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>> records = await ExtractRecords();
        return records;
    }

    private async Task<Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>>> ExtractRecords()
    {
        Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>> records = new()
        {
            { ZendeskRecordType.User, await Search<UserSearchResults>(ZendeskRecordType.User) },
            { ZendeskRecordType.Org, await Search<OrgSearchResults>(ZendeskRecordType.Org) },
            { ZendeskRecordType.Group, await Search<GroupSearchResults>(ZendeskRecordType.Group) },
            { ZendeskRecordType.TicketField, await Search<TicketFieldSearchResults>(ZendeskRecordType.TicketField) },
            { ZendeskRecordType.TicketForm, await Search<TicketFormSearchResults>(ZendeskRecordType.TicketForm) },
            { ZendeskRecordType.Brand, await Search<BrandSearchResults>(ZendeskRecordType.Brand) }
        };
        return records;
    }

    private async Task<List<ZendeskCommonModel>> Search<T>(ZendeskRecordType type) where T : SearchResults
    {
        try
        {
            List<ZendeskCommonModel> allRecords = new List<ZendeskCommonModel>();
            string query = GetQueryByType(type);
            if (string.IsNullOrEmpty(query)) return allRecords;
            while (true)
            {
                T searchResults = await RequestAs<T>(query);
                if (searchResults is null) throw new ArgumentNullException(nameof(searchResults));

                ZendeskCommonModel[] records = searchResults.GetRecords();
                if (records == null || records.Length == 0) break;

                allRecords.AddRange(records);
                string newRoute = searchResults.Next_Page ?? string.Empty;
                if (string.IsNullOrEmpty(newRoute)) break;
                newRoute = newRoute.Replace(_httpClient.BaseAddress!.ToString(), "");
                query = newRoute;
            }
            return allRecords;
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }
    }

    private string GetQueryByType(ZendeskRecordType type) => type switch
    {
        ZendeskRecordType.User => ZendeskQueries.GetUsers,
        ZendeskRecordType.Org => ZendeskQueries.GetOrganizations,
        ZendeskRecordType.Group => ZendeskQueries.GetGroups,
        ZendeskRecordType.TicketForm => ZendeskQueries.GetTicketForms,
        ZendeskRecordType.TicketField => ZendeskQueries.GetTicketFields,
        ZendeskRecordType.Brand => ZendeskQueries.GetBrands,
        _ => throw new NotImplementedException()
    };
}