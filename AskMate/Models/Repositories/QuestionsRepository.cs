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
            var adapter = new NpgsqlDataAdapter("SELECT * FROM questions ORDER BY submission_time ASC", _connection);

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

        public Question GetById(int id)
        {
            try
            {
                _connection.Open();
                var adapter = new NpgsqlDataAdapter("SELECT q.id, q.title, q.description, q.submission_time, a.id AS answer_id, a.message AS answer_message " +
                                                    "FROM questions AS q " +
                                                    "LEFT JOIN answers AS a ON q.id = a.question_id " +
                                                    "WHERE q.id = :id", _connection);
                adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                var table = dataSet.Tables[0];

                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    var question = new Question
                    {
                        Id = (int)row["id"],
                        Title = (string)row["title"],
                        Description = (string)row["description"],
                        PublishedDate = (DateTime)row["submission_time"],
                        Answers = new List<Answer>()
                    };

                    foreach (DataRow answerRow in table.Rows)
                    {
                        // Check if the answer exists (answer_id is not null) and add it to the question's answers list
                        if (!DBNull.Value.Equals(answerRow["answer_id"]))
                        {
                            question.Answers.Add(new Answer
                            {
                                Id = (int)answerRow["answer_id"],
                                Message = (string)answerRow["answer_message"],
                                Question_Id = (int)answerRow["id"],
                                PublishedDate = (DateTime)answerRow["submission_time"]
                            });
                        }
                    }

                    return question;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, and return an appropriate response.
                // For simplicity, let's rethrow the exception here.
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }


    }
}
