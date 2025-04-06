using Test.Application.DTO.BaseResponse;
namespace Test.Application.DTO.Transaction
{
    public class TransactionOutputDTO : BaseResponseDTO
    {
        public long? TotalAmount { get; set; }
        public long? TotalDiscount { get; set; }
        public long? FinalAmount { get; set; }
    }
}
