using System.Text.Json.Serialization;

namespace Portfolio.ETL;

/// <summary>
/// This model represents the json results returned by the Zendesk API when performing
/// a request to the Export Search endpoint. More information at:
/// https://developer.zendesk.com/api-reference/ticketing/ticket-management/search/#export-search-results
/// </summary>
public record ExportSearchResults
{
    /// <summary>
    ///  Tickets found
    /// </summary>
    [JsonPropertyName("results")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public ZendeskTicketModel[]? Results;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    /// <summary>
    /// Url links to the previous and next page of results
    /// </summary>
    [JsonPropertyName("links")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public CursorLinks? Links;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    /// <summary>
    ///  Meta information about the search.
    /// </summary>
    [JsonPropertyName("meta")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public CursorMeta? Meta;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    public class CursorLinks
    {
        /// <summary>
        /// Last page, not in use
        /// </summary>
        [JsonPropertyName("prev")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string? Prev;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        /// <summary>
        ///  URL to the next page of Results
        /// </summary>
        [JsonPropertyName("next")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string? Next;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }

    public class CursorMeta
    {
        /// <summary>
        ///  Boolean indicating if there are more Results
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore;

        /// <summary>
        /// Cursor object returned from the Search Service
        /// </summary>
        [JsonPropertyName("after_cursor")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string? AfterCursor;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }


}