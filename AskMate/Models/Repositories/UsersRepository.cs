﻿using Npgsql;

namespace AskMate.Models.Repositories
{
    public class UsersRepository
    {
        private readonly NpgsqlConnection _connection;

        public UsersRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public int CreateUser(User user)
        {
            _connection.Open();
            int lastInsertId;

            using (var cmd = new NpgsqlCommand(
                       "INSERT INTO users (username, email, password) VALUES (@username, @email, @password) RETURNING id",
                       _connection
                   ))
            {
                cmd.Parameters.AddWithValue("username", user.UserName);
                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("password", user.Password);

                lastInsertId = (int)cmd.ExecuteScalar();
            }

            _connection.Close();

            return lastInsertId;
        }


        public User Login(string usernameOrEmail, string password)
        {
            // Validate the login request
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            User user = null;

            // Create the SQL query to retrieve the user from the database
            var query = "SELECT id, username, email, password FROM users " +
                        "WHERE username = @usernameOrEmail OR email = @usernameOrEmail";

            _connection.Open();

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("usernameOrEmail", usernameOrEmail);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var userId = (int)reader["id"];
                        var username = (string)reader["username"];
                        var email = (string)reader["email"];
                        var passwordHash = (string)reader["password"];

                        user = new User
                        {
                            Id = userId,
                            UserName = username,
                            Email = email
                        };
                    }
                }
            }

            if (user != null)
            {
                // If the user is found and authenticated, create a new LoggedInUser record
                var loggedInUser = new LoggedInUser
                {
                    UserId = user.Id,
                    PublishedDate = DateTime.UtcNow

                };

                // Save the LoggedInUser record in the database
                var insertQuery = "INSERT INTO LoggedInUser (UserId, LoginTime) VALUES (@UserId, @LoginTime)";
                using (var insertCommand = new NpgsqlCommand(insertQuery, _connection))
                {
                    insertCommand.Parameters.AddWithValue("UserId", loggedInUser.UserId);
                    insertCommand.Parameters.AddWithValue("LoginTime", loggedInUser.PublishedDate);
                    // ... (add other parameters if necessary)

                    insertCommand.ExecuteNonQuery();
                }
            }

            _connection.Close();
            return user;
        }

        public void Logout(int userId)
        {
            _connection.Open();

            var deleteQuery = "DELETE FROM LoggedInUser WHERE UserId = @UserId";
            using (var deleteCommand = new NpgsqlCommand(deleteQuery, _connection))
            {
                deleteCommand.Parameters.AddWithValue("UserId", userId);
                deleteCommand.ExecuteNonQuery();
            }

            _connection.Close();
        }




    }
}
