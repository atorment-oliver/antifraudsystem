using Confluent.Kafka;
using ControlKernel.DTO;
using ControlKernel.Events;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TransactionService.Data;

namespace TransactionService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic = "transaction-created";
        private readonly TransactionDbContext _context;
        public TransactionService(TransactionDbContext context)
        {
            _context = context;
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public TransactionService()
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task<TransactionStatusDTO> CreateTransactionAsync(TransactionDTO transactionDTO)
        {
            if (transactionDTO == null)
            {
                throw new ArgumentNullException(nameof(transactionDTO));
            }

            var transaction = new ControlKernel.Models.Transaction
            {
                TransactionExternalId = Guid.NewGuid(),
                SourceAccountId = transactionDTO.SourceAccountId,
                TargetAccountId = transactionDTO.TargetAccountId,
                TransferTypeId = transactionDTO.TransferTypeId,
                Value = transactionDTO.Value,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var transactionCreatedEvent = new TransactionCreatedEvent
            {
                TransactionExternalId = transaction.TransactionExternalId,
                Value = transaction.Value
            };

            var message = new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(transactionCreatedEvent)
            };

            await _producer.ProduceAsync(_topic, message);

            return new TransactionStatusDTO
            {
                TransactionExternalId = transaction.TransactionExternalId,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task<TransactionStatusDTO> GetTransactionAsync(Guid id)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionExternalId == id);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }

            var transactionStatus = new TransactionStatusDTO
            {
                TransactionExternalId = transaction.TransactionExternalId,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt
            };

            return transactionStatus;
        }
    }
}