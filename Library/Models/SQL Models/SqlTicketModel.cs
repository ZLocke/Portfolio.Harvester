using System.ComponentModel.DataAnnotations;

namespace Portfolio.ETL;

/// <summary>
/// Model representing the tranformed data stored in SQL.
/// </summary>
public record SqlTicketModel
{
    [Key] public long Id;
    public string Url;
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
    public string? TicketType;
    public string? Subject;
    public string? Priority;
    public string? Status;
    public string? Recipient;
    public string? RequesterId;
    public string? SubmitterId;
    public string? AssigneeId;
    public string? OrganizationId;
    public string? GroupId;
    public bool? HasIncidents;
    public bool? IsPublic;
    public List<string>? Tags;
    public Dictionary<string, string>? CustomFields;
    public Dictionary<string, string>? Fields;
    public string? TicketFormId;
    public string? BrandId;

    public SqlTicketModel(long id, string url, DateTime createdAt, DateTime updatedAt,
        string? ticketType, string? subject, string? priority, string? status, string? recipient,
        string? requesterId, string? submitterId, string? assigneeId, string? organizationId, string? groupId,
        bool? hasIncidents, bool? isPublic, List<string>? tags, Dictionary<string, string>? customFields,
        Dictionary<string, string>? fields, string? ticketFormId, string? brandId)
    {
        Id = id;
        Url = url;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        TicketType = ticketType;
        Subject = subject;
        Priority = priority;
        Status = status;
        Recipient = recipient;
        RequesterId = requesterId;
        SubmitterId = submitterId;
        AssigneeId = assigneeId;
        OrganizationId = organizationId;
        GroupId = groupId;
        HasIncidents = hasIncidents;
        IsPublic = isPublic;
        Tags = tags;
        CustomFields = customFields;
        Fields = fields;
        TicketFormId = ticketFormId;
        BrandId = brandId;
    }
}

