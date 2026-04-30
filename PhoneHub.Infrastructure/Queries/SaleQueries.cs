namespace PhoneHub.Infrastructure.Queries
{
    public static class SaleQueries
    {
        public static string GetAllWithDetailsMySql = @"
            SELECT s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%Y-%m-%d %H:%i:%s') AS Date,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS CustomerName,
                   u.Email AS CustomerEmail
            FROM sales s
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.IsActive = 1
            ORDER BY s.Date DESC
            LIMIT @Limit;";

        public static string GetAllWithDetailsSqlServer = @"
            SELECT TOP (@Limit)
                   s.Id AS SaleId,
                   CONVERT(VARCHAR, s.Date, 120) AS Date,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS CustomerName,
                   u.Email AS CustomerEmail
            FROM sales s
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.IsActive = 1
            ORDER BY s.Date DESC;";
    }
}
