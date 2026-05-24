namespace PhoneHub.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telephone { get; set; }
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
