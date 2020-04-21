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
        private IConnection _connection;
        private IModel _channel;
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
            
            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(rabbitMqOptions.Exchange, ExchangeType.Topic);
            _channel.QueueDeclare(rabbitMqOptions.Queue, false, false, false, null);
            _channel.QueueBind(rabbitMqOptions.Queue, rabbitMqOptions.Exchange, rabbitMqOptions.Routekey, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(rabbitMqOptions.Queue, false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // string content = JsonConvert.SerializeObject(specialistBindModel);
            CustomServiceLogEvent log = JsonConvert.DeserializeObject<CustomServiceLogEvent>(content);

            switch (log.LogType)
            {
                case LogType.INFO:
                    logger.LogInformation($"Module: {log.ModuleName}, Time: {log.Time}, Message: {log.Message}");
                    break;
                case LogType.ERROR:
                    logger.LogError($"Module: {log.ModuleName}, Time: {log.Time}, Message: {log.Message}");
                    break;
            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
