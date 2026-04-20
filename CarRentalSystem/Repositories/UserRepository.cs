using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using CarRentalSystem.Models;
using CarRentalSystem.Database;

namespace CarRentalSystem.Repositories
{
    public class UserRepository
    {
        // Fetches a user from the database by their username (Used for Login)
        public UserModel GetUserByUsername(string username)
        {
            using (IDbConnection db = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
                // Dapper maps the SQL result directly to your UserModel!
                return db.QueryFirstOrDefault<UserModel>(query, new { Username = username });
            }
        }

        // Inserts a new user into the database (Used for Owner/Admin registering staff)
        public void CreateUser(UserModel user)
        {
            using (IDbConnection db = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string query = @"
                    INSERT INTO Users (Username, PasswordHash, Role, FirstName, LastName, IsActive)
                    VALUES (@Username, @PasswordHash, @Role, @FirstName, @LastName, @IsActive)";

                db.Execute(query, user);
            }
        }
    }
}