namespace PhoneHub.Services.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string hashedPassword, string plainPassword);
    }
}
