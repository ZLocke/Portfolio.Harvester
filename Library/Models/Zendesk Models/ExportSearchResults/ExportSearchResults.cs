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
    public ZendeskTicketModel[] Results { get; set; }


    /// <summary>
    /// Url links to the previous and next page of results
    /// </summary>
    [JsonPropertyName("links")]
    public CursorLinks Links { get; set; }


    /// <summary>
    ///  Meta information about the search.
    /// </summary>
    [JsonPropertyName("meta")]
    public CursorMeta Meta { get; set; }


    public class CursorLinks
    {
        /// <summary>
        /// Last page, not in use
        /// </summary>
        [JsonPropertyName("prev")]
        public string Prev { get; set; }


        /// <summary>
        ///  URL to the next page of Results
        /// </summary>
        [JsonPropertyName("next")]
        public string Next { get; set; }

    }

    public class CursorMeta
    {
        /// <summary>
        ///  Boolean indicating if there are more Results
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        /// <summary>
        /// Cursor object returned from the Search Service
        /// </summary>
        [JsonPropertyName("after_cursor")]
        public string AfterCursor { get; set; }

    }


}