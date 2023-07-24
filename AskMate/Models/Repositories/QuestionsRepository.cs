using Npgsql;

namespace AskMate.Models.Repositories
{
    public class QuestionsRepository
    {
        private readonly NpgsqlConnection _connection;

        public QuestionsRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

    }
}
