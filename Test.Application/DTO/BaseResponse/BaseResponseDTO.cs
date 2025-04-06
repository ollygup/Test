using System.ComponentModel.DataAnnotations;

namespace Test.Application.DTO.BaseResponse
{
    public class BaseResponseDTO
    {
        [Required]
        public int Result { get; set; }
        public string? ResultMessage { get; set; }
    }
}
