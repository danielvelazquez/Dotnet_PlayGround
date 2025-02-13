using CosmosDBEmulator.Contracts;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace CosmosDBEmulator
{
    class Program
    {
        private static readonly string Endpoint = "https://localhost:8081";
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        static async Task Main(string[] args)
        {
            try
            {
                using (CosmosClient client = new CosmosClient(Endpoint, PrimaryKey))
                {
                    Database database = await client.CreateDatabaseIfNotExistsAsync("Personas");
                    Container container = await database.CreateContainerIfNotExistsAsync("Persona", "/ExternalId");
                    //var collection = client.CreateDocumentCollectionIfNotExistsAsync(database.SelfLink, new DocumentCollection { Id = "Persona" }).Result.Resource;
                    var persona = new Persona
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
                    if (!string.IsNullOrEmpty(persona.Id))
                    {
                        await container.CreateItemAsync(persona, new PartitionKey(persona.PartitionKey));
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
