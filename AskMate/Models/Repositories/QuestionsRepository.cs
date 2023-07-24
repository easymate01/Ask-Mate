using Npgsql;
using System.Data;


namespace AskMate.Models.Repositories
{
    public class QuestionsRepository
    {
        private readonly NpgsqlConnection _connection;

        public QuestionsRepository(NpgsqlConnection connection)
        {
            _connection = connection;

        }


        public List<Question> GetAll()
        {
            _connection.Open();
            var adapter = new NpgsqlDataAdapter("SELECT * FROM questions", _connection);

            var dataSet = new DataSet();
            adapter.Fill(dataSet);
            var table = dataSet.Tables[0];

            var queryResult = new List<Question>();
            foreach (DataRow row in table.Rows)
            {
                queryResult.Add(new Question
                {
                    Id = (int)row["id"],
                    Title = (string)row["title"],
                    Description = (string)row["description"],
                    PublishedDate = (DateTime)row["submission_time"]
                });
            }

            _connection.Close();

            return queryResult;
        }
    }
}
