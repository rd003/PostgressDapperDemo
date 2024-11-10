using Dapper;
using Npgsql;
using PostgressDapperDemo.Models.Domain;

namespace PostgressDapperDemo.Repositories;

public class PersonRepository : IPersonRepository
{
    private string _connectionString = "";
    private IConfiguration _configuration;
    public PersonRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("default");
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var createdId = await connection.ExecuteScalarAsync<int>("INSERT INTO person (name, email) VALUES (@name, @email); SELECT LASTVAL();", person);
        person.Id = createdId;
        return person;
    }

    public async Task<Person> UpdatePersonAsync(Person person)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("UPDATE person SET name = @name, email = @email WHERE id = @id", person);
        return person;
    }

    public async Task DeletePersonAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE FROM person WHERE id = @id", new { id });
    }

    public async Task<IEnumerable<Person>> GetAllPersonAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Person>("SELECT * FROM person");
    }

    public async Task<Person?> GetPersonByIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Person>("SELECT * FROM person WHERE id = @id", new { id });
    }
}

