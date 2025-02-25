namespace CosmosDBEmulator.Services
{
    public interface IGetPersonas
    {
        Task GetPersonaAsync(string name);
        void GetPersona(string name);
        void GetPersona(string name, string query = "");
    }
}
