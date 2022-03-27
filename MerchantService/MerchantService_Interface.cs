using MerchantService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantService
{
    public interface IMerchantService
    {
        List<IMerchantModel> GetAll();
        IMerchantModel AddMerchant(IMerchantModel newMerchant);
        KTransOutcomeInfo UpdateMerchant(IMerchantModel victim);
        IMerchantModel GetById(int id);
        KTransOutcomeInfo DeleteMerchant(IMerchantModel victim);
    }
}
