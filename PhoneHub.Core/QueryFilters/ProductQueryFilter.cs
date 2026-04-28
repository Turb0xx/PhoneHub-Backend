namespace PhoneHub.Core.QueryFilters
{
    public class ProductQueryFilter
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? OnlyAvailable { get; set; }
    }
}
