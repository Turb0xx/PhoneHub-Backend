namespace PhoneHub.Infrastructure.Queries
{
    public static class InvoiceQueries
    {
        public static string GetByIdMySql = @"
            SELECT i.Id AS InvoiceId,
                   i.InvoiceNumber,
                   DATE_FORMAT(i.IssuedAt, '%Y-%m-%d %H:%i:%s') AS IssuedAt,
                   s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%Y-%m-%d %H:%i:%s') AS SaleDate,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail
            FROM invoices i
            INNER JOIN sales s ON i.SaleId = s.Id
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE i.Id = @Id;";

        public static string GetByIdSqlServer = @"
            SELECT i.Id AS InvoiceId,
                   i.InvoiceNumber,
                   CONVERT(VARCHAR, i.IssuedAt, 120) AS IssuedAt,
                   s.Id AS SaleId,
                   CONVERT(VARCHAR, s.Date, 120) AS SaleDate,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail
            FROM invoices i
            INNER JOIN sales s ON i.SaleId = s.Id
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE i.Id = @Id;";

        public static string GetBySaleIdMySql = @"
            SELECT i.Id AS InvoiceId,
                   i.InvoiceNumber,
                   DATE_FORMAT(i.IssuedAt, '%Y-%m-%d %H:%i:%s') AS IssuedAt,
                   s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%Y-%m-%d %H:%i:%s') AS SaleDate,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail
            FROM invoices i
            INNER JOIN sales s ON i.SaleId = s.Id
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE i.SaleId = @SaleId;";

        public static string GetBySaleIdSqlServer = @"
            SELECT i.Id AS InvoiceId,
                   i.InvoiceNumber,
                   CONVERT(VARCHAR, i.IssuedAt, 120) AS IssuedAt,
                   s.Id AS SaleId,
                   CONVERT(VARCHAR, s.Date, 120) AS SaleDate,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail
            FROM invoices i
            INNER JOIN sales s ON i.SaleId = s.Id
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE i.SaleId = @SaleId;";
    }
}
