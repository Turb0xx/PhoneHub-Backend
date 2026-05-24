namespace PhoneHub.Infrastructure.Queries
{
    public static class SaleQueries
    {
        public static string GetByIdWithDetailsMySql = @"
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
            WHERE s.Id = @Id;";

        public static string GetByIdWithDetailsSqlServer = @"
            SELECT s.Id AS SaleId,
                   CONVERT(VARCHAR, s.Date, 120) AS Date,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS CustomerName,
                   u.Email AS CustomerEmail
            FROM sales s
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.Id = @Id;";

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

        // RN-10: reporte solo considera ventas con IsActive = 1
        public static string GetDailyReportMySql = @"
            SELECT s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail,
                   COUNT(s.Id) AS SalesCount,
                   SUM(s.TotalAmount) AS SubTotal
            FROM sales s
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.IsActive = 1
              AND DATE(s.Date) = DATE(@Date)
            GROUP BY s.UserId, u.FirstName, u.LastName, u.Email
            ORDER BY SubTotal DESC;";

        public static string GetDailyReportSqlServer = @"
            SELECT s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail,
                   COUNT(s.Id) AS SalesCount,
                   SUM(s.TotalAmount) AS SubTotal
            FROM sales s
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.IsActive = 1
              AND CAST(s.Date AS DATE) = CAST(@Date AS DATE)
            GROUP BY s.UserId, u.FirstName, u.LastName, u.Email
            ORDER BY SubTotal DESC;";
    }
}
