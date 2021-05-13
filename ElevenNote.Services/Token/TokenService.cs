using System.Threading.Tasks;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace ElevenNote.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public TokenService(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<TokenResponse> GetTokenAsync(TokenRequest model)
        {
            var user = await GetValidUserAsync(model);
            if (user is null)
            {
                return null;
            }
            return GenerateToken(user);
        }

        private async Task<UserEntity> GetValidUserAsync(TokenRequest model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.UserName.ToLower() == model.UserName.ToLower());
            if (user is null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<UserEntity>();
            var verifyPasswordResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (verifyPasswordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return user;
        }

        private TokenResponse GenerateToken(UserEntity user)
        {
            var claims = GetClaims(user);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenResponse = new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                IssuedAt = token.ValidFrom,
                Expires = token.ValidTo
            };

            return tokenResponse;
        }

        private Claim[] GetClaims(UserEntity user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var name = !string.IsNullOrWhiteSpace(fullName) ? fullName : user.UserName;

            var claims = new Claim[]
            {
                new Claim("Id",user.Id.ToString()),
                new Claim("Username",user.UserName),
                new Claim("Email", user.Email),
                new Claim("Name",name)
            };

            return claims;
        }
    }
}