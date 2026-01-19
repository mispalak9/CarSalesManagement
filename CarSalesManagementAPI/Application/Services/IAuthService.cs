using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> Login(LoginDto loginDto);
    Task<ApiResponse<LoginResponseDto>> GetUserInfo(int userId);
}
