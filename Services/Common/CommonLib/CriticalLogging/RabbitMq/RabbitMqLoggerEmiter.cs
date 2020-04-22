using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Logging
{
    public class RabbitMqLoggerEmiter
    {
        private IConnection connection;
        private IModel channel;
        private readonly RabbitMqOptions rabbitMqOptions;

        public RabbitMqLoggerEmiter(RabbitMqOptions rabbitMqOptions)
        {            
            ConnectionFactory factory = new ConnectionFactory() 
            { 
                HostName = rabbitMqOptions.HostName, 
                Port = rabbitMqOptions .Port
            }; 

            this.rabbitMqOptions = rabbitMqOptions;
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            this.channel.QueueBind(rabbitMqOptions.Queue, rabbitMqOptions.Exchange, rabbitMqOptions.Routekey, null);
        }

        public void Send(string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            if (channel.IsClosed == false)
                channel.BasicPublish(
                    exchange: rabbitMqOptions.Exchange,
                    routingKey: rabbitMqOptions.Routekey,
                    basicProperties: null,
                    body: body);
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();            
        }
    }
}
