using Dapper;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;
using CarSalesManagementAPI.Infrastructure.Data;
using System.Data;

namespace CarSalesManagementAPI.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuthRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"SELECT u.*, s.SalesmanID, s.SalesmanName, s.SalesmanCode, s.Email, s.Phone
                    FROM Users u
                    LEFT JOIN Salesmen s ON u.SalesmanID = s.SalesmanID
                    WHERE u.Username = @Username AND u.IsActive = 1";

        var result = await connection.QueryAsync<User, Salesman?, User>(
            sql,
            (user, salesman) =>
            {
                user.Salesman = salesman;
                return user;
            },
            new { Username = username },
            splitOn: "SalesmanID"
        );

        return result.FirstOrDefault();
    }

    public async Task<User?> GetUserById(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"SELECT u.*, s.SalesmanID, s.SalesmanName, s.SalesmanCode, s.Email, s.Phone
                    FROM Users u
                    LEFT JOIN Salesmen s ON u.SalesmanID = s.SalesmanID
                    WHERE u.UserID = @UserId AND u.IsActive = 1";

        var result = await connection.QueryAsync<User, Salesman?, User>(
            sql,
            (user, salesman) =>
            {
                user.Salesman = salesman;
                return user;
            },
            new { UserId = userId },
            splitOn: "SalesmanID"
        );

        return result.FirstOrDefault();
    }

    public async Task<bool> UpdateLastLoginDate(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "UPDATE Users SET LastLoginDate = GETDATE() WHERE UserID = @UserId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId });
        
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Role>> GetUserRoles(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"SELECT r.* 
                    FROM Roles r
                    INNER JOIN UserRoles ur ON r.RoleID = ur.RoleID
                    WHERE ur.UserID = @UserId AND r.IsActive = 1";

        return await connection.QueryAsync<Role>(sql, new { UserId = userId });
    }
}
