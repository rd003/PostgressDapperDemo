using System;

namespace PostgressDapperDemo.Models.DTOs;

public class PersonDisplayDto
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
