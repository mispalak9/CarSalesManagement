using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CarSalesManagementAPI.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Username and password are required.",
                    Errors = new List<string> { "Please provide both username and password." }
                };
            }

            var user = await _authRepository.GetUserByUsername(loginDto.Username);
            if (user == null)
            {
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Invalid credentials.",
                    Errors = new List<string> { "Username or password is incorrect." }
                };
            }

            var passwordHash = ComputeHash(loginDto.Password);
            if (user.PasswordHash != passwordHash)
            {
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Invalid credentials.",
                    Errors = new List<string> { "Username or password is incorrect." }
                };
            }

            // Update last login date
            await _authRepository.UpdateLastLoginDate(user.UserID);

            // Get user roles
            var roles = await _authRepository.GetUserRoles(user.UserID);

            var response = new LoginResponseDto
            {
                UserID = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.Select(r => r.RoleName).ToList(),
                Token = null
            };

            return new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", loginDto.Username);
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Error during login.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<LoginResponseDto>> GetUserInfo(int userId)
    {
        try
        {
            var user = await _authRepository.GetUserById(userId);
            if (user == null)
            {
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "User not found.",
                    Errors = new List<string> { $"User with ID {userId} not found." }
                };
            }

            var roles = await _authRepository.GetUserRoles(userId);

            var response = new LoginResponseDto
            {
                UserID = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.Select(r => r.RoleName).ToList(),
                Token = null
            };

            return new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "User information retrieved successfully.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Error retrieving user information.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private static string ComputeHash(string password)
    {
        // Simple SHA256 hash for now - in production use BCrypt or Argon2
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
