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
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? type;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? subject;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? raw_subject;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? priority;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? status;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? recipient;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public long? requester_id;
    public long? submitter_id;
    public long? assignee_id;
    public long? organization_id;
    public long? group_id;
    public bool? has_incidents;
    public bool? is_public;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string[]? tags;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public Field[] custom_fields { get; init; }
    public Field[] fields { get; init; }
    public long? ticket_form_id;
    public long? brand_id;

    public record Field
    {
        public long id { get; init; }
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public object? value { get; init; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
