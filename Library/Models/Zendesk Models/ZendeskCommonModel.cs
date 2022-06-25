using System.Text.Json.Serialization;

namespace Portfolio.ETL;

/// <summary>
/// A lightweight model used for sideloading data. Only has properties that all Zendesk items have.
/// Sideloaded data is used to join the results of multiple requests to populate id fields with their actual names.
/// </summary>
public record ZendeskCommonModel
{
    [JsonPropertyName("id")] public long Id;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("name")] public string? Name;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("created_at")] public DateTime Created_at;
    [JsonPropertyName("updated_at")] public DateTime Updated_at;
}