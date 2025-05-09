namespace TavernSystem.Mocels.Models;

public class Adventurer
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Race { get; set; }
    public string ExperienceLevel { get; set; }
    public Person PersonData { get; set; }
}