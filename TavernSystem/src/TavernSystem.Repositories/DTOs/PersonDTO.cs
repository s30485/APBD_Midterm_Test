﻿namespace TavernSystem.Repositories.DTOs;

public class PersonDTO
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public bool HasBounty { get; set; }
}