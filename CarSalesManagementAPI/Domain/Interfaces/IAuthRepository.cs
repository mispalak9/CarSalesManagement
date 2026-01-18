using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> UpdateLastLoginDateAsync(int userId);
    Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
}
