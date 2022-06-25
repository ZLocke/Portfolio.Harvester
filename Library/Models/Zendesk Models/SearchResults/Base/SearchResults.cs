using System.Text.Json.Serialization;

namespace Portfolio.ETL;

/// <summary>
/// This is the base class for all models that represent search results returned by
/// the Zendesk API. More information on the Zendesk API Search can be found here:
/// https://developer.zendesk.com/api-reference/ticketing/ticket-management/search/
/// </summary>
public abstract class SearchResults
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("next_page")] public string? Next_Page { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("previous_page")] public string? Prev { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("count")] public int? Count { get; set; }

    /// <summary>
    /// Gets the records for the child type.
    /// </summary>
    /// <returns>Array of Zendesk Common Models used for joining request results.</returns>
    public abstract ZendeskCommonModel[] GetRecords();
}
