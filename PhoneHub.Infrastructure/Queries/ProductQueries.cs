namespace PhoneHub.Infrastructure.Queries
{
    public static class ProductQueries
    {
        public static string GetAllMySql = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            ORDER BY Brand ASC;";

        public static string GetAllSqlServer = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            ORDER BY Brand ASC;";

        public static string GetByIdMySql = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Id = @Id;";

        public static string GetByIdSqlServer = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Id = @Id;";

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
