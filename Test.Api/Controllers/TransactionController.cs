using Microsoft.AspNetCore.Mvc;
using Test.Application.DTO.Transaction;
using Test.Application.Services.IServices;

namespace Test.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase 
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost(Name = "Transaction")]
        public async Task<TransactionOutputDTO> Post(TransactionInputDTO input)
        {
            try
            {
                var result = await _transactionService.SubmitTransactionAsync(input);
                return result;
            }
            catch (Exception ex)
            {
                var err = new TransactionOutputDTO
                {
                    Result = 0,
                    ResultMessage = "Error detected, please try again."
                };
                return err;
            }
        }
    }
}
