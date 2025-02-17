using Microsoft.Azure.Cosmos;
using CosmosDBEmulator.Contracts;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDBEmulator.Services
{
    public class GetPersonas : IGetPersonas
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
            QueryRequestOptions options = new()
            {
                // MaxConcurrency especifica el número de operaciones simultáneas ejecutadas en el lado cliente durante la ejecución de consultas en paralelo.
                // Si se establece en 1, el paralelismo se deshabilita de forma eficaz. Si se establece en -1, el SDK administra este valor.
                // Lo ideal sería establecer este valor en el número de particiones físicas del contenedor.
                MaxItemCount = 500,
                // La propiedad MaxBufferedItemCount establece el número máximo de elementos almacenados en búfer en el lado cliente
                // durante la ejecución de una consulta en paralelo. Si se establece en -1, el SDK administra este valor.
                // El valor ideal para esta configuración dependerá en gran medida de las características de la máquina cliente.
                MaxBufferedItemCount = 5000
            };

            // This might not work because the container is not initialized
            // Also, the query is case sensitive, so making to upper while executing might not find the record

            // TODO: Review continuation token
            var query3 = _container.GetItemLinqQueryable<Persona>(true, "", options).Where(i => i.Name == name).ToFeedIterator<Persona>();
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
