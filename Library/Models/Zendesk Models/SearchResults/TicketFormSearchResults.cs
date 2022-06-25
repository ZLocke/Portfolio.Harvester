using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class TicketFormSearchResults : SearchResults
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("ticket_forms")] public ZendeskCommonModel[]? Ticket_Forms;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonConstructor]
    public TicketFormSearchResults(ZendeskCommonModel[] ticket_forms, string next_page, string prev, int count)
    {
        Ticket_Forms = ticket_forms;
        Next_Page = next_page;
        Prev = prev;
        Count = count;
    }
    public override ZendeskCommonModel[] GetRecords() => Ticket_Forms;
}

