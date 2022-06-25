namespace Portfolio.ETL;

/// <summary>
/// Routes for Zendesk API that return records of a specific model type
/// </summary>
public static class ZendeskQueries
{
    public const string GetTicketFields = "ticket_fields";
    public const string GetUsers = "users";
    public const string GetOrganizations = "organizations";
    public const string GetTicketForms = "ticket_forms";
    public const string GetBrands = "brands";
    public const string GetGroups = "groups";
}