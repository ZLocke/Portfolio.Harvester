﻿using System.Text.Json.Serialization;

namespace Portfolio.ETL;

public class BrandSearchResults : SearchResults
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [JsonPropertyName("brands")] public ZendeskCommonModel[]? Brands;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
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

