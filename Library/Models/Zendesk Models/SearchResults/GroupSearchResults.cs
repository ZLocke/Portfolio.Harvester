using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class GroupSearchResults : SearchResults
{

    [JsonPropertyName("groups")] public ZendeskCommonModel[] Groups;

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

