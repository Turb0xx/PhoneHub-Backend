namespace PhoneHub.Core.CustomEntities
{
    public class PasswordOptions
    {
        public string SaltKey { get; set; } = null!;
        public int Iterations { get; set; }
    }
}
