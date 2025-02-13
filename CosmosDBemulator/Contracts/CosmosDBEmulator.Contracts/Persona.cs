namespace CosmosDBEmulator.Contracts
{
    public class Persona
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public Roles AssignedRole { get; set; }
        // TODO: Revisar si partitionkey debe ser publico o privado
        public string PartitionKey => ExternalId;
    }
}
