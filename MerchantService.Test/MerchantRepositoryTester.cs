using KDBAbstractions.Repository;
using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MerchantService.Test
{
    public class MerchantRepositoryTester
    {
        AKDBAbstraction aKDBAbstraction;
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
            IMerchantModel newMerchant = new MerchantModel { Name = $@"herman testing service''s {firstCount}" ,Address="77 test lane , Brampton ON L6Y 1LT", Phone="437-123-1234"};
            IMerchantModel addedMerchant = kRepository.Record(newMerchant);
            merchants = kRepository.GetAll();
            int secondCount = merchants.Count;

            Assert.NotZero(addedMerchant.Id);
            Assert.Greater(secondCount, firstCount);
        }


        [Test]
        public void TestUpdatingAMerchant()
        {

            IKRepository<IMerchantModel> kRepository = new MerchantRepository(aKDBAbstraction);

            List<IMerchantModel> merchants = kRepository.GetAll();
            int firstCount = merchants.Count;
            IMerchantModel newMerchant = new MerchantModel { Name = $@"herman testing service''s {firstCount}", Address = "77 test lane , Brampton ON L6Y 1LT", Phone = "437-123-1234" };
            IMerchantModel addedRecord =  kRepository.Record(newMerchant);
            merchants = kRepository.GetAll();
         //   int secountCount = merchants.Count;

        //    merchants[secountCount-1].Id

            kRepository.UpdateRecord(new MerchantModel { Id = addedRecord.Id, Name = $"updated record {firstCount}", Phone = "416-123-3549" });

            IMerchantModel updatedRecord = kRepository.GetById(addedRecord.Id);

            Assert.IsNotNull(updatedRecord);
            Assert.AreNotEqual(updatedRecord, addedRecord);
            Assert.AreNotEqual(updatedRecord.Phone, addedRecord.Phone);
            //int secondCount = merchants.Count;
        }



        [Test]
        public void TestDeletingMerchantJustAdded()
        {

            IKRepository<IMerchantModel> kRepository = new MerchantRepository(aKDBAbstraction);

            List<IMerchantModel> merchants = kRepository.GetAll();
            int firstCount = merchants.Count;
            IMerchantModel newMerchant = new MerchantModel { Name = $@"herman testing service''s {firstCount}", Address = "77 test lane , Brampton ON L6Y 1LT", Phone = "437-123-1234" };
            IMerchantModel addedRecord = kRepository.Record(newMerchant);
            merchants = kRepository.GetAll();

            int secondCount = merchants.Count;
            Assert.Greater( secondCount, firstCount);
            kRepository.DeleteRecord(addedRecord);

            merchants = kRepository.GetAll();
            int thirdCount = merchants.Count;
            Assert.AreEqual(firstCount, thirdCount);
        }

        [Test]
        public void TestDeletingMerchantThatHasExpenses()
        {
            try
            {
                IKRepository<IMerchantModel> kRepository = new MerchantRepository(aKDBAbstraction);

                List<IMerchantModel> merchants = kRepository.GetAll();
                int firstCount = merchants.Count;
                IMerchantModel earlyMerchantThatIsProbablyUsed =  merchants[0];

                kRepository.DeleteRecord(earlyMerchantThatIsProbablyUsed);
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Expenses are related to this. wont delete", ex.Message);
            }

        }

    }
}