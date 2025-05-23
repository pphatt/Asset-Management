using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        public AuthService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                StaffCode = user.StaffCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                JoinedDate = user.JoinedDate,
                Type = user.Type.ToString(),
                IsPasswordUpdated = user.IsPasswordUpdated,
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Username or password is incorrect. Please try again");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("Your account is disabled. Please contact with IT Team");
            }

            var passwordComparisonResult = _passwordHasher.VerifyPassword(loginRequest.Password, user.Password);
            if (!passwordComparisonResult)
            {
                throw new UnauthorizedAccessException("Username or password is incorrect. Please try again");
            }

            var token = GenerateJwtToken(user);
            var userInfo = MapToUserDto(user);

            return new LoginResponse
            {
                AccessToken = token,
                UserInfo = userInfo
            };
        }

        public bool VerifyToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(string username, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (!string.IsNullOrEmpty(request.Password) && !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (_passwordHasher.VerifyPassword(request.NewPassword, user.Password))
            {
                throw new InvalidOperationException("New password must be different from the old password");
            }

            user.Password = _passwordHasher.HashPassword(request.NewPassword);
            user.IsPasswordUpdated = true;
            _userRepository.Update(user);

            var token = GenerateJwtToken(user);
            var userInfo = MapToUserDto(user);

            return new ChangePasswordResponse
            {
                AccessToken = token,
                UserInfo = userInfo
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Type.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationMinutes = _configuration.GetValue<int>("Jwt:TokenExpirationInMinutes", 60);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}