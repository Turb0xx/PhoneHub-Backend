using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhoneHub.Core.Entities;
using PhoneHub.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhoneHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public TokenController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            var user = await _userService.GetByCredentialsAsync(userLogin);
            if (user == null)
                return Unauthorized("Credenciales inválidas.");

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private string GenerateToken(Core.Entities.User user)
        {
            // HEADER
            var symmetricKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]!));
            var signingCredentials = new SigningCredentials(
                symmetricKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // PAYLOAD
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", $"{user.FirstName} {user.LastName}"),
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(8)
            );

            // FIRMA
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
