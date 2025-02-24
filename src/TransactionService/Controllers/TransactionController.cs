using ControlKernel.DTO;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Services;

namespace TransactionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            var result = await _transactionService.CreateTransactionAsync(transactionDTO);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            var result = await _transactionService.GetTransactionAsync(id);
            return Ok(result);
        }
    }
}