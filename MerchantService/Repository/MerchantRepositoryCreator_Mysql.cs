using KDBAbstractions.Repository;
using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantService
{

    public class MerchantRepositoryCreator_Mysql:IMerchantRepositoryCreator
    {
        public MerchantRepositoryCreator_Mysql() { }
        public MerchantRepositoryArgs Args { get; set; }

        public IKRepository<IMerchantModel> Create()
        {
            string connString = Args.ConnectionString;
            AKDBAbstraction dbAbstraction = new KMysql_KDBAbstraction(connString);
            IKRepository<IMerchantModel> kRepository = new MerchantRepository(dbAbstraction);
            return kRepository;
        }

    }
}
