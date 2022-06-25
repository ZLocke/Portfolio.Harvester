using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class TicketFormSearchResults : SearchResults
{

    [JsonPropertyName("ticket_forms")] public ZendeskCommonModel[] Ticket_Forms;

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

