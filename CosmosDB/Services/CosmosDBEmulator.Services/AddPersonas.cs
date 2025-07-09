using CosmosDBEmulator.Contracts;
using Microsoft.Azure.Cosmos;

namespace CosmosDBEmulator.Services
{
    public class AddPersonas : IAddPersonas
    {
        public async Task AddPersonaAsync(Persona persona)
        {
            try
            {
                string endpoint = "";
                string primaryKey = "";
                using (CosmosClient client = new CosmosClient(endpoint, primaryKey))
                {
                    Database database = await client.CreateDatabaseIfNotExistsAsync("Personas");
                    Container container = await database.CreateContainerIfNotExistsAsync("Persona", "/ExternalId");
                    //var collection = client.CreateDocumentCollectionIfNotExistsAsync(database.SelfLink, new DocumentCollection { Id = "Persona" }).Result.Resource;
                    var nuevaPersona = new Persona
                    {
                        Id = Guid.NewGuid().ToString(),
                        ExternalId = "123",
                        Name = "John",
                        LastName = "Doe",
                        //AssignedRole = new Roles
                        //{
                        //    Id = Guid.NewGuid(),
                        //    Name = "Admin",
                        //    Description = "Admin role"
                        //}
                    };
                    if (!string.IsNullOrEmpty(nuevaPersona.Id))
                    {
                        await container.CreateItemAsync(nuevaPersona, new PartitionKey(nuevaPersona.PartitionKey));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
