using Microsoft.Azure.Cosmos;
using CosmosDBEmulator.Contracts;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDBEmulator.Services
{
    public class GetPersonas
    {
        private Container _container;

        //TODO: Does container need to be public?
        public GetPersonas()
        {

        }

        /// <summary>
        /// Gets a person by name.
        /// Retrieves async query results withouth blocking other operations.
        /// </summary>
        /// <param name="id"></param>
        public async Task GetPersonaAsync(string name)
        {
            // This might not work because the container is not initialized
            // Also, the query is case sensitive, so making to upper while executing might not find the record
            var query3 = _container.GetItemLinqQueryable<Persona>().Where(i => i.Name == name).ToFeedIterator<Persona>();
        }

        /// <summary>
        /// blocks other operations until the query is executed.
        /// </summary>
        /// <param name="name"></param>
        public void GetPersona(string name)
        {
            // Option 1:
            // This option blocks other operations until the query is executed.
            var query = _container.GetItemLinqQueryable<Persona>()
                .Where(p => p.Name.ToUpper() == name.ToUpper()).ToList<Persona>();
        }


        /// <summary>
        /// This option uses a query definition to make the query case insensitive using the UPPER function.
        /// TODO: Update function
        /// </summary>
        /// <param name="name"></param>
        /// <param name="query"></param>
        public void GetPersona(string name, string query = "")
        {
            var query2 = new QueryDefinition("SELECT * FROM c WHERE UPPER(c.Name) = @name")
                .WithParameter("@name", name.ToUpper());
        }
    }
}
