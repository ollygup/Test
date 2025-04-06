using Test.Application.DTO.Transaction;
namespace Test.Application.Services.IServices
{
    public interface ITransactionService
    {
        Task<TransactionOutputDTO> SubmitTransactionAsync(TransactionInputDTO input);
    }
}
