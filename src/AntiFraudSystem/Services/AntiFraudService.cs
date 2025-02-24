// AntiFraudService/Services/AntiFraudService.cs
using Confluent.Kafka;
using ControlKernel.DTO;
using ControlKernel.Events;
using System.Text.Json;

namespace AntiFraudService.Services
{
    public class AntiFraudService : IHostedService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _statusTopic = "transaction-status-updated";
        private Task _executingTask;
        private CancellationTokenSource _cancellationTokenSource;

        public AntiFraudService()
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _executingTask = ExecuteAsync(_cancellationTokenSource.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "anti-fraud-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("transaction-created");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(cancellationToken);
                    var transactionEvent = JsonSerializer.Deserialize<TransactionCreatedEvent>(result.Message.Value);

                    var status = ValidateTransaction(transactionEvent);

                    var statusEvent = new TransactionStatusUpdatedEvent
                    {
                        TransactionExternalId = transactionEvent.TransactionExternalId,
                        Status = status
                    };

                    var message = new Message<Null, string>
                    {
                        Value = JsonSerializer.Serialize(statusEvent)
                    };

                    await _producer.ProduceAsync(_statusTopic, message);
                }
            }
        }

        private string ValidateTransaction(TransactionCreatedEvent transaction)
        {
            if (transaction.Value > 2000)
            {
                return "rejected";
            }

            return "approved";
        }
    }
}