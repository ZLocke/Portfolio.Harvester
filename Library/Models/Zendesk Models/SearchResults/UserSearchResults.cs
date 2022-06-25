using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class UserSearchResults : SearchResults
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("users")] public ZendeskCommonModel[]? Users;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    [JsonConstructor]
    public UserSearchResults(ZendeskCommonModel[] users, string next_page, string prev, int count)
    {
        Users = users;
        Next_Page = next_page;
        Prev = prev;
        Count = count;
    }

    public override ZendeskCommonModel[] GetRecords() => Users;
}

