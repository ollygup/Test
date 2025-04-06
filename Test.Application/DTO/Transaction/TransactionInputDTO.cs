using System.ComponentModel.DataAnnotations;
using Test.Application.DTO.Item;

namespace Test.Application.DTO.Transaction
{
    public class TransactionInputDTO
    {
        public string PartnerKey { get; set; }
        public string PartnerRefNo { get; set; }
        public string PartnerPassword { get; set; }
        public long TotalAmount { get; set; }
        public List<ItemDTO> Items { get; set; }
        public string Timestamp { get; set; }
        public string Sig { get; set; }
    }
}
