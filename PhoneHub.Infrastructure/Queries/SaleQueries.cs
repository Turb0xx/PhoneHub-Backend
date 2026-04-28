namespace PhoneHub.Infrastructure.Queries
{
    public static class SaleQueries
    {
        public static string GetAllWithDetails = @"
            SELECT Id, ProductId, UserId, Quantity, TotalAmount, Date, IsActive
            FROM sales
            WHERE IsActive = 1
            ORDER BY Date DESC;";
    }
}
