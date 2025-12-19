using Microsoft.Testing.Platform.Configurations;

namespace Playground.RabbitMQ.Tests
{
    [TestClass]
    public class MessageSenderServiceIntegration : BaseIntegrationTest
    {
        private IMessageSenderService _messageSenderService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            BaseTestInit();
            IntegrationsUtility.RegisterServices(_serviceCollection, Configuration);
        }
        [TestInitialize]
        public void TestInit()
        {
            _provider = _serviceCollection.BuildServiceProvider();
            _messageSenderService = _provider.GetRequiredService<IMessageSenderService>()
                ?? throw new InvalidOperationException("Konami Player Service is not registered in the service provider.");
        }

        [TestMethod]
        public async Task WhenSendingMessage_Success()
        {
            // Arrange
            var queueName = "konami.players.queue";
            var message = new { Text = "Hello, RabbitMQ!" };

            // Act
            await _messageSenderService.SendMessageAsync(queueName, message);

            // Assert
            // TODO: How to validate the message is in the queue?
        }
    }
}
