using Npgsql;

namespace AskMate.Models.Repositories
{
    public class AnswersRepository
    {
        private readonly NpgsqlConnection _connection;

        public AnswersRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public int Create(Answer answer, int id)
        {
            _connection.Open();
            int newAnswerId = 0;

            using (var command = new NpgsqlCommand("SELECT COUNT(userid) FROM loggedinuser WHERE userid = @id", _connection))
            {
                command.Parameters.AddWithValue("id", id);
                int userCount = Convert.ToInt32(command.ExecuteScalar());

                if (userCount > 0)
                {
                    // The user with the given id exists in the loggedinuser table.
                    using (var cmd = new NpgsqlCommand(
                       "INSERT INTO answers (message, question_id, submission_time) VALUES (@message, @questionId, @submissionTime) RETURNING id",
                       _connection
                   ))
                    {
                        cmd.Parameters.AddWithValue("message", answer.Message);
                        cmd.Parameters.AddWithValue("questionId", answer.Question_Id);
                        cmd.Parameters.AddWithValue("submissionTime", answer.PublishedDate);

                        newAnswerId = (int)cmd.ExecuteScalar();
                    }
                }
                else
                {
                    // The user with the given id does not exist in the loggedinuser table.
                    Console.WriteLine("A megadott id nem található a loggedinuser táblában.");
                }
            }
            _connection.Close();
            return newAnswerId;
        }



        public void Delete(int id)
        {
            _connection.Open();
            var adapter = new NpgsqlDataAdapter(
                "DELETE FROM answears WHERE id = :id",
                _connection
            );
            adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

            adapter.SelectCommand?.ExecuteNonQuery();
            _connection.Close();
        }
    }
}
