namespace PhoneHub.Infrastructure.Queries
{
    public static class UserQueries
    {
        public static string GetAllMySql =
            @"SELECT Id, FirstName, LastName, Email, Role, Telephone, IsActive
              FROM users
              ORDER BY Id";

        public static string GetAllSqlServer =
            @"SELECT Id, FirstName, LastName, Email, Role, Telephone, IsActive
              FROM users
              ORDER BY Id";

        public static string GetByIdMySql =
            @"SELECT Id, FirstName, LastName, Email, Role, Telephone, IsActive
              FROM users
              WHERE Id = @Id";

        public static string GetByIdSqlServer =
            @"SELECT Id, FirstName, LastName, Email, Role, Telephone, IsActive
              FROM users
              WHERE Id = @Id";
    }
}
