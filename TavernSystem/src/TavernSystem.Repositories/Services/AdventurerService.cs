using System.Text.RegularExpressions;
using TavernSystem.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using TavernSystem.Mocels.Models;
using TavernSystem.Repositories.DTOs;


namespace TavernSystem.Repositories.Services;

public class AdventurerService : IAdventurerService
{
    private readonly string _connectionString;

    public AdventurerService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Adventurer>> GetAllAdventurersAsync()
    {
        var adventurers = new List<Adventurer>();
        string query = @"SELECT 
    a.Id, 
    a.Nickname, 
    r.Name AS RaceName, 
    e.Name AS ExperienceLevelName, 
    p.Id, 
    p.FirstName, 
    p.MiddleName, 
    p.LastName, 
    p.HasBounty
FROM Adventurer a
JOIN Person p ON a.PersonId = p.Id
JOIN Race r ON a.RaceId = r.Id
JOIN ExperienceLevel e ON a.ExperienceId = e.Id";
        
        

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            adventurers.Add(new Adventurer
            {
                Id = reader.GetInt32(0),
                Nickname = reader.GetString(1),
                Race = reader.GetString(2),
                ExperienceLevel = reader.GetString(3),
                PersonData = new Person
                {
                    Id = reader.GetString(4),
                    FirstName = reader.GetString(5),
                    MiddleName = reader.IsDBNull(6) ? null : reader.GetString(6),
                    LastName = reader.GetString(7),
                    HasBounty = reader.GetBoolean(8)
                }
            });

        }

        return adventurers;
    }

    public async Task<Adventurer?> GetAdventurerByIdAsync(int id)
    {
        string query = @"
        SELECT 
            a.Id, 
            a.Nickname, 
            r.Name AS RaceName, 
            e.Name AS ExperienceLevelName, 
            p.Id, 
            p.FirstName, 
            p.MiddleName, 
            p.LastName, 
            p.HasBounty
        FROM Adventurer a
        JOIN Person p ON a.PersonId = p.Id
        JOIN Race r ON a.RaceId = r.Id
        JOIN ExperienceLevel e ON a.ExperienceId = e.Id
        WHERE a.Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Adventurer
            {
                Id = reader.GetInt32(0),
                Nickname = reader.GetString(1),
                Race = reader.GetString(2),
                ExperienceLevel = reader.GetString(3),
                PersonData = new Person
                {
                    Id = reader.GetString(4),
                    FirstName = reader.GetString(5),
                    MiddleName = reader.IsDBNull(6) ? null : reader.GetString(6),
                    LastName = reader.GetString(7),
                    HasBounty = reader.GetBoolean(8)
                }
            };
        }

        return null;
    }


    public async Task<bool> CreateAdventurerAsync(CreateAdventurerDTO dto)
    {
    if (!IsValidPersonId(dto.PersonId))
        throw new ArgumentException("Invalid Person ID format.");

    using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync();
    using var transaction = connection.BeginTransaction();

    try
    {
        //if person exists
        var personCheckCmd = new SqlCommand(
            "SELECT HasBounty FROM Person WHERE Id = @PersonId", connection, transaction);
        personCheckCmd.Parameters.AddWithValue("@PersonId", dto.PersonId);
        var hasBountyObj = await personCheckCmd.ExecuteScalarAsync();

        if (hasBountyObj == null)
        {
            throw new InvalidOperationException("Person not found.");
        }

        if ((bool)hasBountyObj)
        {
            throw new InvalidOperationException("Person has a bounty.");
        }

        //if to check adventurer already exists for this person
        var advCheckCmd = new SqlCommand(
            "SELECT COUNT(*) FROM Adventurer WHERE PersonId = @PersonId", connection, transaction);
        advCheckCmd.Parameters.AddWithValue("@PersonId", dto.PersonId);
        var exists = (int)await advCheckCmd.ExecuteScalarAsync() > 0;
        if (exists)
        {
            return false; //registered before
        }

        //insert new adventurer
        var insertCmd = new SqlCommand(@"
            INSERT INTO Adventurer (Nickname, RaceId, ExperienceId, PersonId)
            VALUES (@Nickname, @RaceId, @ExperienceId, @PersonId)", connection, transaction);

        insertCmd.Parameters.AddWithValue("@Nickname", dto.Nickname);
        insertCmd.Parameters.AddWithValue("@RaceId", dto.RaceId);
        insertCmd.Parameters.AddWithValue("@ExperienceId", dto.ExperienceLevelId);
        insertCmd.Parameters.AddWithValue("@PersonId", dto.PersonId);

        await insertCmd.ExecuteNonQueryAsync();
        await transaction.CommitAsync();

        return true;
    }
    catch (InvalidOperationException)
    {
        await transaction.RollbackAsync();
        throw; // Let controller handle 403
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}


//id validation
private bool IsValidPersonId(string id)
{
    if (string.IsNullOrEmpty(id) || id.Length != 18)
        return false;

    string pattern = @"^[A-Z]{2}(?<year>\d{4})(?<month>0[1-9]|1[0-1])(?<day>0[1-9]|1[0-9]|2[0-8])\d{4}[A-Z]{2}$"; //taken from stackoverflow xd
    return Regex.IsMatch(id, pattern);
}

}