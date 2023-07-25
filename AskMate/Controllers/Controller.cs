using AskMate.Models;
using AskMate.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{

    //private readonly string _connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=1234;Database=AskMate";
    private readonly string _connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=12345;Database=AskMate";

    [HttpGet]
    public IActionResult GetAll()
    {
        var repository = new QuestionsRepository(new NpgsqlConnection(_connectionString));
        return Ok(repository.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var repository = new QuestionsRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.GetById(id));
    }

    [HttpPost("/Question")]
    public IActionResult Create(Question question)
    {
        var repository = new QuestionsRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.Create(question));
    }

    [HttpPost("/Answer")]
    public IActionResult Addanswer(Answer answer)
    {
        var repository = new AnswersRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.AddAnswer(answer));
    }

    [HttpDelete("/Question/{id}")]
    public IActionResult DeleteQuestion(int id)
    {
        var repository = new QuestionsRepository(new NpgsqlConnection(_connectionString));
        repository.Delete(id);

        return Ok("The Question has been deleted");
    }
    [HttpDelete("/Answer/{id}")]
    public IActionResult DeleteAnswear(int id)
    {
        var repository = new AnswersRepository(new NpgsqlConnection(_connectionString));
        repository.Delete(id);

        return Ok("The Answear has been deleted");
    }


    [HttpPost("/User")]
    public IActionResult CreateUser(User user)
    {
        var repository = new UsersRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.CreateUser(user));
    }

    [HttpPost("/User/Login")]
    public IActionResult Login([FromBody] User user)
    {
        var repository = new UsersRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.Login(user.UserName, user.Password));
    }

    [HttpPost("/User/Logout")]
    public IActionResult Logout()
    {
        
        HttpContext.Session.Clear(); 

        return Ok("Logged out successfully.");
    }

}