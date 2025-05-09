using TavernSystem.Mocels.Models;
using TavernSystem.Repositories.DTOs;

namespace TavernSystem.Repositories.Interfaces;

public interface IAdventurerService
{
    Task<List<Adventurer>> GetAllAdventurersAsync();
    Task<Adventurer?> GetAdventurerByIdAsync(int id);
    Task<bool> CreateAdventurerAsync(CreateAdventurerDTO dto);


}
