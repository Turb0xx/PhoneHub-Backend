namespace PhoneHub.Infrastructure.Queries
{
    public static class ProductQueries
    {
        public static string GetAllAvailableMySql = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Stock > 0
            ORDER BY Brand ASC
            LIMIT @Limit;";

        public static string GetAllAvailableSqlServer = @"
            SELECT TOP (@Limit) Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Stock > 0
            ORDER BY Brand ASC;";
    }
}
