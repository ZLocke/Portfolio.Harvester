using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class BrandSearchResults : SearchResults
{

    [JsonPropertyName("brands")] public ZendeskCommonModel[] Brands;

    [JsonConstructor]
    public BrandSearchResults(ZendeskCommonModel[] brands, string next_page, string prev, int count)
    {
        Brands = brands;
        Next_Page = next_page;
        Prev = prev;
        Count = count;
    }
    public override ZendeskCommonModel[] GetRecords() => Brands;
}

