using Npgsql;

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

            // Create a connection to the PostgreSQL database
            _connection.Open();

            // Create the SQL query to retrieve the user from the database
            var query = "SELECT id, username, email, password" +
                        "FROM users " +
                        "WHERE username = @username OR email = @usernameOrEmail";

            using var command = new NpgsqlCommand(query, _connection);
            command.Parameters.AddWithValue("@usernameOrEmail", usernameOrEmail);

            using var reader = command.ExecuteReader();

            // Check if the user exists and if the password matches the stored hashed password
            if (!reader.Read())
            {
                _connection.Close();
                return null;
            }

            var userId = (int)reader["id"];
            var username = (string)reader["username"];
            var email = (string)reader["email"];

            // Close the connection and return the user with the JWT token
            _connection.Close();
            return new User
            {
                Id = userId,
                UserName = username,
                Email = email,
                Password = password
            };
        }

    }
}
