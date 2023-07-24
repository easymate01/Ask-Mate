using AskMate.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{

    private readonly string _connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=1234;Database=AskMate";
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
}