using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class TicketFieldSearchResults : SearchResults
{

    [JsonPropertyName("ticket_fields")] public ZendeskCommonModel[] Ticket_Fields;

    [JsonConstructor]
    public TicketFieldSearchResults(ZendeskCommonModel[] ticket_fields, string next_page, string prev, int count)
    {
        Ticket_Fields = ticket_fields;
        Next_Page = next_page;
        Prev = prev;
        Count = count;
    }
    public override ZendeskCommonModel[] GetRecords() => Ticket_Fields;
}

