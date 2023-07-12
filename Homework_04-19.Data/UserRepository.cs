using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_04_19.Data
{
    public class UserRepository
    {
        private string _conStr;

        public UserRepository(string connectionString)
        {
            _conStr = connectionString;
        }

        public void InsertAd(Ad ad)
        {
            var connection = new SqlConnection(_conStr);
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Ads(UserId, DateCreated, Phone, Description) " +
                "VALUES(@userId, @date, @phoneNum, @descript)";
            command.Parameters.AddWithValue("@userId", ad.UserId);
            command.Parameters.AddWithValue("@date", DateTime.Today);
            command.Parameters.AddWithValue("@phoneNum", ad.Phone);
            command.Parameters.AddWithValue("@descript", ad.Description);
            connection.Open();

            command.ExecuteNonQuery();
        }

        public List<Ad> GetAds()
        {
            var connection = new SqlConnection(_conStr);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT a.*, UserName AS Name FROM Ads a " +
                "JOIN Users u " +
                "ON u.Id = a.UserId";
            connection.Open();

            var reader = command.ExecuteReader();

            return FromReader(reader);
        }

        public List<Ad> GetAdsForUser(int userId)
        {
            var connection = new SqlConnection(_conStr);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT a.*, UserName AS Name FROM Ads a " +
                "JOIN Users u " +
                "ON u.Id = a.UserId " +
                "WHERE a.UserId = @id";
            command.Parameters.AddWithValue("@id", userId);
            connection.Open();

            var reader = command.ExecuteReader();

            return FromReader(reader);
        }

        public List<Ad> FromReader(SqlDataReader reader)
        {
            List<Ad> ads = new();

            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    Name = (string)reader["Name"],
                    Phone = (string)reader["Phone"],
                    Description = (string)reader["Description"]
                });
            }

            return ads;
        }

        public void NewUser(User user, string password)
        {
            var connection = new SqlConnection(_conStr);
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users(UserName, UserEmail, PasswordHash) " +
                "VALUES(@name, @email, @hash)";

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            command.Parameters.AddWithValue("@name", user.UserName);
            command.Parameters.AddWithValue("@email", user.UserEmail);
            command.Parameters.AddWithValue("@hash", passwordHash);
            connection.Open();

            command.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetUserByEmail(email);
            if(user == null)
            {
                return null;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isValid)
            {
                return null;
            }

            return user;
        }

        public User GetUserByEmail(string email)
        {
            var connection = new SqlConnection(_conStr);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE UserEmail = @email";
            command.Parameters.AddWithValue("@email", email);
            connection.Open();

            var reader = command.ExecuteReader();
           
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                UserName = (string)reader["UserName"],
                UserEmail = (string)reader["UserEmail"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
    }
}
