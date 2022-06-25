using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class GroupSearchResults : SearchResults
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("groups")] public ZendeskCommonModel[]? Groups;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonConstructor]
    public GroupSearchResults(ZendeskCommonModel[] groups, string next_page, string prev, int count)
    {
        Groups = groups;
        Next_Page = next_page;
        Prev = prev;
        Count = count;
    }
    public override ZendeskCommonModel[] GetRecords() => Groups;
}

