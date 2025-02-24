using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ControlKernel.Events;
using ControlKernel.Models;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TransactionService.Data;

namespace TransactionService.Services
{
    public class TransactionStatusConsumer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Task _executingTask;
        private CancellationTokenSource _cancellationTokenSource;

        public TransactionStatusConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
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
                GroupId = "transaction-status-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("transaction-status-updated");

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(cancellationToken);
                        var statusEvent = JsonSerializer.Deserialize<TransactionStatusUpdatedEvent>(result.Message.Value);

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();

                            var transaction = await context.Transactions
                                .FirstOrDefaultAsync(t => t.TransactionExternalId == statusEvent.TransactionExternalId);

                            if (transaction != null)
                            {
                                transaction.Status = statusEvent.Status;
                                await context.SaveChangesAsync();
                                Console.WriteLine($"Transacción {transaction.TransactionExternalId} actualizada a {statusEvent.Status}");
                            }
                            else
                            {
                                Console.WriteLine($"Transacción {statusEvent.TransactionExternalId} no encontrada");
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error al consumir mensaje: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error inesperado: {e.Message}");
                    }
                }
            }
        }
    }
}
