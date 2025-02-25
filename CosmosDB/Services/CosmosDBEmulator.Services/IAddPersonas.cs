using CosmosDBEmulator.Contracts;
namespace CosmosDBEmulator.Services
{
    public interface IAddPersonas
    {
        Task AddPersonaAsync(Persona persona);
        void AddPersona(Persona persona);
    }
}
