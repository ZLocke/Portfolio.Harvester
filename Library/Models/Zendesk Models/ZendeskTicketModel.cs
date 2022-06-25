namespace Portfolio.ETL;

/// <summary>
/// This model represents the json schema used by the Zendesk API for tickets. 
/// Not all properties of Tickets are captured here, as some properties were deemed superfluous.
/// Zendesk API Tickets Documentation: https://developer.zendesk.com/api-reference/ticketing/tickets/tickets/
/// </summary>
public record ZendeskTicketModel
{
    public long id;
    public string url;
    public DateTime created_at;
    public DateTime updated_at;
    public string? type;
    public string? subject;
    public string? priority;
    public string? status;
    public string? recipient;
    public long? requester_id;
    public long? submitter_id;
    public long? assignee_id;
    public long? organization_id;
    public long? group_id;
    public bool? has_incidents;
    public bool? is_public;
    public string[]? tags;
    public Field[] custom_fields { get; init; }
    public Field[] fields { get; init; }
    public long? ticket_form_id;
    public long? brand_id;

    public record Field
    {
        public long id { get; init; }
        public object? value { get; init; }

    }
}
