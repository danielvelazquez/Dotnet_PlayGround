using CosmosDBEmulator.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace PersonasWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonasController : ControllerBase
{
    private readonly ILogger<PersonasController> _logger;

    public PersonasController(ILogger<PersonasController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name ="GetPersonas")]
    public IEnumerable<Persona> Get()
    {
        return new List<Persona>();
    }
}
