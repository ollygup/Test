using Test.Core.Domain;
using Test.Core.IRepository;

namespace Test.Infrastructure.Repository
{
    public class PartnerRepository : IPartnerRepository
    {
        public PartnerRepository() { }

        public Task<List<Partner>> GetAllAsync()
        {
            var data = new List<Partner>();

            data.Add(new Partner { PartnerRefNo = "FG-00001", PartnerKey = "FAKEGOOGLE", PartnerPassword = "FAKEPASSWORD1234" });
            data.Add(new Partner { PartnerRefNo = "FG-00002", PartnerKey = "FAKEPEOPLE", PartnerPassword = "FAKEPASSWORD4578" });

            return Task.FromResult(data);
        }
    }
}
