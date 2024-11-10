using Microsoft.AspNetCore.Mvc;
using PostgressDapperDemo.Mappers;
using PostgressDapperDemo.Models.DTOs;
using PostgressDapperDemo.Repositories;

namespace PostgressDapperDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController : ControllerBase
{
    private readonly IPersonRepository _personRepository;
    private readonly ILogger<PeopleController> _logger;

    public PeopleController(IPersonRepository personRepository, ILogger<PeopleController> logger)
    {
        _personRepository = personRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePerson(PersonCreateDto personCreateDto)
    {
        try
        {
            var person = personCreateDto.ToPerson();
            await _personRepository.CreatePersonAsync(person);
            return CreatedAtRoute(nameof(GetPerson), new { id = person.Id }, person.ToPersonDisplay());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating person: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonUpdateDto person)
    {
        try
        {
            if (id != person.Id)
            {
                return BadRequest("Id mismatch");
            }
            var existingPerson = await _personRepository.GetPersonByIdAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }
            var personToUpdate = person.ToPerson();
            await _personRepository.UpdatePersonAsync(personToUpdate);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating person: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        try
        {
            var person = await _personRepository.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            await _personRepository.DeletePersonAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting person: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}", Name = "GetPerson")]
    public async Task<IActionResult> GetPerson(int id)
    {
        try
        {
            var person = await _personRepository.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person.ToPersonDisplay());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting person: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPeople()
    {
        try
        {
            var people = await _personRepository.GetAllPersonAsync();
            var peopleToReturn = people.Select(p => p.ToPersonDisplay()).ToList();
            return Ok(peopleToReturn);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all people: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
