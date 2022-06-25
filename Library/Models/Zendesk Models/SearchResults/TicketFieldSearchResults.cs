using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class TicketFieldSearchResults : SearchResults
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("ticket_fields")] public ZendeskCommonModel[]? Ticket_Fields;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
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

