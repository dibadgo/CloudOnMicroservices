using EventBus.RabbitMQ.Standard.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Service.Handlers
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private ILogger<ConsumeRabbitMQHostedService> logger;
        private IConnection connection;
        private IModel channel;
        private readonly RabbitMqOptions rabbitMqOptions;

        public ConsumeRabbitMQHostedService(ILogger<ConsumeRabbitMQHostedService> logger, RabbitMqOptions rabbitMqOptions)
        {
            this.logger = logger;
            this.rabbitMqOptions = rabbitMqOptions;

            InitRabbitMQ();
        }
        
        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = rabbitMqOptions.HostName, Port = rabbitMqOptions.Port };
            
            connection = factory.CreateConnection();

            channel = connection.CreateModel();

            channel.ExchangeDeclare(rabbitMqOptions.Exchange, ExchangeType.Topic);
            channel.QueueDeclare(rabbitMqOptions.Queue, false, false, false, null);
            channel.QueueBind(rabbitMqOptions.Queue, rabbitMqOptions.Exchange, rabbitMqOptions.Routekey, null);
            channel.BasicQos(0, 1, false);

            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                HandleMessage(content);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            channel.BasicConsume(rabbitMqOptions.Queue, false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            if (content.StartsWith("Error"))
                logger.LogError(content);
            else if (content.StartsWith("ritical"))
                logger.LogCritical(content);
            else
                logger.LogDebug(content);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            channel.Close();
            connection.Close();
            base.Dispose();
        }
    }
}
