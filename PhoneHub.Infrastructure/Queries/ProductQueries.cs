namespace PhoneHub.Infrastructure.Queries
{
    public static class ProductQueries
    {
        public static string GetAllAvailable = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Stock > 0
            ORDER BY Brand ASC;";
    }
}
