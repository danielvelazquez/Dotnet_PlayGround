using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Playground.RabbitMQ.Function.Servicios
{
    public class MessageSenderService : IMessageSenderService
    {
        private readonly ILogger<MessageSenderService> _logger;
        private readonly IConnectionFactory _connectionFactory;

        public MessageSenderService(
            ILogger<MessageSenderService> logger,
            IConnectionFactory connectionFactory
            )
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Sends the message to the specified RabbitMQ queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync<T>(string queueName, T message)
        {
            try
            {
                // Crea la conexión y el canal usando el factory inyectado
                using var connection = await _connectionFactory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                // Declara la cola (idempotente)
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // Serializa el mensaje a JSON
                //var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(message.ToString());
                var props = new BasicProperties();

                // Publica el mensaje


                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: props, // Use the created properties
                    body: body,
                    cancellationToken: new CancellationToken());

                //_logger.LogInformation($"Mensaje enviado a la cola '{queueName}': {json}");
                _logger.LogInformation($"Mensaje enviado a la cola '{queueName}'");

                await Task.CompletedTask; // Para cumplir con la firma async
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar mensaje a la cola '{queueName}': {ex.Message}");
                throw; // Re-lanzar la excepción para que pueda ser manejada por el consumidor
            }
            //catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
            //{
            //    await Task.Delay(5000);
            //    // apply retry logic
            //}
        }
    }
}
