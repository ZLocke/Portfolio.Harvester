using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class OrgSearchResults : SearchResults
{
    [JsonPropertyName("organizations")] public ZendeskCommonModel[] Organizations;
    [JsonConstructor]
    public OrgSearchResults(ZendeskCommonModel[] organizations, string next_page, string prev, int count)
    {
        Organizations = organizations;
        Next_Page = next_page;
        Prev = prev;
        Count = count;
    }

    public override ZendeskCommonModel[] GetRecords() => Organizations;
}

