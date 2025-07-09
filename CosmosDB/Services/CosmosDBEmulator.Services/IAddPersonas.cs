using CosmosDBEmulator.Contracts;
namespace CosmosDBEmulator.Services
{
    public interface IAddPersonas
    {
        Task AddPersonaAsync(Persona persona);
    }
}
