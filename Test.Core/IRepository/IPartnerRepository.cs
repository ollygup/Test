using Test.Core.Domain;
namespace Test.Core.IRepository
{
    public interface IPartnerRepository
    {
        Task<List<Partner>> GetAllAsync();
    }
}
