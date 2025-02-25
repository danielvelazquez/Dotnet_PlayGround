using CosmosDBEmulator.Contracts;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDBEmulator.Services
{
    public class AddPersonas : IAddPersonas
    {
        public void AddPersona(Persona persona)
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

        public Task AddPersonaAsync(Persona persona)
        {
            throw new NotImplementedException();
        }
    }
}
