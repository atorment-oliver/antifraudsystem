using ControlKernel.DTO;
namespace TransactionService.Services
{
    public interface ITransactionService
    {
        Task<TransactionStatusDTO> CreateTransactionAsync(TransactionDTO transactionDTO);
        Task<TransactionStatusDTO> GetTransactionAsync(Guid id);
    }
}
