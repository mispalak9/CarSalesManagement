using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByUsername(string username);
    Task<User?> GetUserById(int userId);
    Task<bool> UpdateLastLoginDate(int userId);
    Task<IEnumerable<Role>> GetUserRoles(int userId);
}
