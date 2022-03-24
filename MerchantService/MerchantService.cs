using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantService
{
    public class ServiceFacade: IMerchantService
    {
        private IKRepository<IMerchantModel> kRepository;

        public ServiceFacade(IKRepository<IMerchantModel> kRepository)
        {
            this.kRepository = kRepository;
        }

        public IMerchantModel AddMerchant(IMerchantModel newMerchant)
        {
            IMerchantModel recordedRecord =  this.kRepository.Record(newMerchant);
            return recordedRecord;
        }

        public KTransOutcomeInfo DeleteMerchant(IMerchantModel victim)
        {
          
            KTransOutcomeInfo result = new KTransOutcomeInfo();
            result.RecordsAffectedCount = kRepository.DeleteRecord(victim);
            return result;
        }

        public List<IMerchantModel> GetAll()
        {
            var result = kRepository.GetAll();
            return result;
        }

        public IMerchantModel GetById(int id)
        {
            var result = kRepository.GetAll(id)[0];
            return result;
        }

        public KTransOutcomeInfo UpdateMerchant(MerchantModel victim)
        {
            KTransOutcomeInfo result = new KTransOutcomeInfo();
            result.RecordsAffectedCount = kRepository.UpdateRecord(victim);
            return result;
        }
    }
}
