using PostgressDapperDemo.Models.Domain;
using PostgressDapperDemo.Models.DTOs;

namespace PostgressDapperDemo.Mappers;

public static class PersonMapper
{
    public static Person ToPerson(this PersonCreateDto person)
    {
        return new Person
        {
            Name = person.Name,
            Email = person.Email
        };
    }

    public static Person ToPerson(this PersonUpdateDto person)
    {
        return new Person
        {
            Id = person.Id,
            Name = person.Name,
            Email = person.Email
        };
    }

    public static PersonDisplayDto ToPersonDisplay(this Person person)
    {
        return new PersonDisplayDto
        {
            Id = person.Id,
            Name = person.Name,
            Email = person.Email
        };
    }
}
