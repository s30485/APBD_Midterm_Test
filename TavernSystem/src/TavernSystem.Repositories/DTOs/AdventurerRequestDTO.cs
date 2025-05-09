using TavernSystem.Mocels.Models;

namespace TavernSystem.Repositories.DTOs;

public class AdventurerRequestDTO
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Race { get; set; }
    public string ExperienceLevel { get; set; }
    public List<Person> PersonData { get; set; }
}