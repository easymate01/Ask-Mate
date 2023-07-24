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

    }
}
