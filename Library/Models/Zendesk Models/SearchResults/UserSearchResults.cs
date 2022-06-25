using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class UserSearchResults : SearchResults
{

    [JsonPropertyName("users")] public ZendeskCommonModel[] Users;


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

