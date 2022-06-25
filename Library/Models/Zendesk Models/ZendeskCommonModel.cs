using System.Text.Json.Serialization;

namespace Portfolio.ETL;

/// <summary>
/// A lightweight model used for sideloading data. Only has properties that all Zendesk items have.
/// Sideloaded data is used to join the results of multiple requests to populate id fields with their actual names.
/// </summary>
public record ZendeskCommonModel
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}