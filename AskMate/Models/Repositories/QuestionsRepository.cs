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


        public int Create(Question question, int id)
        {
            _connection.Open();
            int lastInsertId = 0;

            using (var cmd = new NpgsqlCommand("SELECT COUNT(userid) FROM loggedinuser WHERE userid = @id", _connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (userCount > 0)
                {
                    // The user with the given id exists in the loggedinuser table.
                    using (var cmdInsertQuestion = new NpgsqlCommand("INSERT INTO questions (title, description) VALUES (@title, @description) RETURNING id", _connection))
                    {
                        cmdInsertQuestion.Parameters.AddWithValue("title", question.Title);
                        cmdInsertQuestion.Parameters.AddWithValue("description", question.Description);

                        lastInsertId = Convert.ToInt32(cmdInsertQuestion.ExecuteScalar());
                    }
                }
                else
                {
                    // The user with the given id does not exist in the loggedinuser table.
                    Console.WriteLine("A megadott id nem található a loggedinuser táblában.");
                }
            }

            _connection.Close();

            return lastInsertId;
        }






        public void Delete(int id)
        {
            _connection.Open();
            var adapter = new NpgsqlDataAdapter(
                "DELETE FROM questions WHERE id = :id",
                _connection
            );
            adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

            adapter.SelectCommand?.ExecuteNonQuery();
            _connection.Close();
        }

        // SEARCH
        public List<Question> Search(string searchPhrase)
        {
            try
            {
                _connection.Open();

                var adapter = new NpgsqlDataAdapter(
                    "SELECT q.id, q.title, q.description, q.submission_time, a.id AS answer_id, a.message AS answer_message " +
                    "FROM questions AS q " +
                    "LEFT JOIN answers AS a ON q.id = a.question_id " +
                    "WHERE q.title ILIKE '%' || :searchPhrase || '%' OR " +
                    "q.description ILIKE '%' || :searchPhrase || '%' OR " +
                    "a.message ILIKE '%' || :searchPhrase || '%'",
                    _connection
                );

                adapter.SelectCommand?.Parameters.AddWithValue(":searchPhrase", searchPhrase);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                var table = dataSet.Tables[0];

                var questions = new List<Question>();

                foreach (DataRow row in table.Rows)
                {
                    var questionId = (int)row["id"];
                    var question = questions.FirstOrDefault(q => q.Id == questionId);

                    if (question == null)
                    {
                        question = new Question
                        {
                            Id = questionId,
                            Title = (string)row["title"],
                            Description = (string)row["description"],
                            PublishedDate = (DateTime)row["submission_time"],
                            Answers = new List<Answer>()
                        };

                        questions.Add(question);
                    }

                    if (!DBNull.Value.Equals(row["answer_id"]))
                    {
                        question.Answers.Add(new Answer
                        {
                            Id = (int)row["answer_id"],
                            Message = (string)row["answer_message"],
                            Question_Id = questionId,
                            PublishedDate = (DateTime)row["submission_time"]
                        });
                    }
                }

                return questions;
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
