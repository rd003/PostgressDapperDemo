using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PostgressDapperDemo.Controllers;
using PostgressDapperDemo.Mappers;
using PostgressDapperDemo.Models.Domain;
using PostgressDapperDemo.Models.DTOs;
using PostgressDapperDemo.Repositories;

namespace PostgressDapperDemo.UnitTests;

public class PeopleControllerTests
{
    private readonly IPersonRepository _personRepository;
    private readonly ILogger<PeopleController> _logger;
    private readonly PeopleController _controller;

    List<Person> people = [
        new Person { Id = 1, Name = "john", Email = "john@gmail.com" },
        new Person { Id = 2, Name = "john2", Email = "john2@gmail.com" },
        ];

    public PeopleControllerTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _logger = Substitute.For<ILogger<PeopleController>>();
        _controller= new PeopleController(_personRepository,_logger);
    }

    [Fact]
    public async Task GetAllPeople_ReturnsOkWithPeoplesList()
    {
        // Arrange
        _personRepository.GetAllPersonAsync().Returns(people);

        // Act
        var result = await _controller.GetAllPeople();

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var peopleList = Assert.IsType<List<PersonDisplayDto>>(okObjectResult.Value);
        Assert.NotNull(peopleList);
        Assert.NotEmpty(peopleList);      
    }

    [Fact]
    public async Task CreatePerson_Returns_CreatedWithRoute_WithObject()
    {
        // arrange
        var person = people.First();
        _personRepository.CreatePersonAsync(person).Returns(person);

        // act
        var personToCreate = new PersonCreateDto { Email = person.Email, Name = person.Name };
        var result = await _controller.CreatePerson(personToCreate);

        // assert
        var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal(nameof(_controller.GetPerson), createdAtRouteResult.RouteName);
        //Assert.Equal(1, createdAtRouteResult.RouteValues["id"]);
        var createdPerson = Assert.IsType<PersonDisplayDto>(createdAtRouteResult.Value);
        Assert.NotNull(createdPerson);
        Assert.Equal(personToCreate.Name, createdPerson.Name);
        Assert.Equal(personToCreate.Email, createdPerson.Email);
    }

    [Fact]
    public async Task CreatePerson_Returns500StatusCode_WhenThrowsException()
    {
        // Arrange
        var personToCreate = new PersonCreateDto { Email = "john@gmail.com", Name = "john" };

        _personRepository.CreatePersonAsync(Arg.Any<Person>()).Throws<Exception>();

        // Act
        var result = await _controller.CreatePerson(personToCreate);

        // Assert
        var statusCodeResult= Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePerson_Returns_NoContent()
    {
        // Arrange
        var person = people.First();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);
        _personRepository.UpdatePersonAsync(Arg.Any<Person>()).Returns(person);

        // Act
        var personToUpdate = new PersonUpdateDto { Id=person.Id, Name=person.Name,Email=person.Email };
       var result = await _controller.UpdatePerson(person.Id,personToUpdate);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePerson_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        int id = 3;
        var personToUpdate = new PersonUpdateDto { Id=1, Name="John", Email="john@example.com" };

        // Act
        var result = await _controller.UpdatePerson(id,personToUpdate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePerson_ReturnsNotFound_WhenPersonNotFound()
    {
        // Arrange
        int id = 3;
        _personRepository.GetPersonByIdAsync(id).Returns((Person)null);

        // Act
        var personToUpdate = new PersonUpdateDto { Id = 3, Name = "John", Email = "john@gmail.com" };
        var result = await _controller.UpdatePerson(id, personToUpdate);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // arrange
        _personRepository.GetPersonByIdAsync(3).Returns((Person)null);

        // act
        var response = await _controller.DeletePerson(3);

        // assert
        var notFoundResult = Assert.IsType<NotFoundResult>(response);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_ReturnsNoContent_WhenSuccess()
    {
        // arrange
        var person = people.First();
        _personRepository.GetPersonByIdAsync(3).Returns(person);

        // act
        var response = await _controller.DeletePerson(3);

        // assert
        var noContentResult = Assert.IsType<NoContentResult>(response);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    

}