using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;

namespace MerchantService
{
    public interface IMerchantRepositoryCreator
    {
        MerchantRepositoryArgs Args { get; set; }
        IKRepository<IMerchantModel> Create();
    }
}