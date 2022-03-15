using KDBAbstractions.Repository;
using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace MerchantService.Test
{
    public class MerchantRepositoryTester
    {
        AKDBAbstraction aKDBAbstraction;
        int testCount = 1;
        [SetUp]
        public void Setup()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("kExpenseConfig.json", optional: false, reloadOnChange: false).Build();

            string connString = config["connectionString_test"].ToString();
            aKDBAbstraction =new KMysql_KDBAbstraction(connString);
        }

        [Test]
        public void TestGettingAllMerchants()
        {
            IKRepository<IMerchantModel> kRepository = new MerchantRepository(aKDBAbstraction);

            List<IMerchantModel> merchants =  kRepository.GetAll();

            Assert.Greater(merchants.Count, 0);
            Assert.Pass();
        }

        [Test]
        public void TestAddingAMerchant()
        {
            IKRepository<IMerchantModel> kRepository = new MerchantRepository(aKDBAbstraction);

            List<IMerchantModel> merchants = kRepository.GetAll();
            int firstCount = merchants.Count;
            IMerchantModel newMerchant = new MerchantModel { Name = $"herman testing services {firstCount}" };
            kRepository.Record(newMerchant);
            merchants = kRepository.GetAll();
            int secondCount = merchants.Count;


            Assert.Greater(secondCount, firstCount);
        }
    }
}