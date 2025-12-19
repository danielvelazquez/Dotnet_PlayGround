using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.RabbitMQ.Function.Servicios
{
    public interface IMessageSenderService
    {
        public Task SendMessageAsync<T>(string queueName, T message);
    }
}
