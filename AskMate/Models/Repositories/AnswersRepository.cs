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

        public int AddAnswer(Answer answer)
        {
            _connection.Open();

            int newAnswerId;

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

            _connection.Close();

            return newAnswerId;
        }
    }
}
