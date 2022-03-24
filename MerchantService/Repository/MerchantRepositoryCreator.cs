using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantService
{

    public class MerchantRepositoryCreator 
    {
        private KInstanciater instanctr;

        public static MerchantRepositoryCreator Instanciate { get {
                var result = new MerchantRepositoryCreator();
                return result;
            
            } }
        private MerchantRepositoryCreator()
        {
          this.instanctr =   new KInstanciater(this.GetType());
        }

        public IKRepository<IMerchantModel> Create(MerchantRepositoryArgs args)
        {
            string className = $"MerchantRepositoryCreator_{args.RepositoryType}";
            instanctr.ClassName = className;

            IMerchantRepositoryCreator subCreator =(IMerchantRepositoryCreator) instanctr.Instanciate();
            subCreator.Args = args;
            var result =   subCreator.Create();
            return result;
        }
    }
}
