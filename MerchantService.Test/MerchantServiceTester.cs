using KDBAbstractions.Repository;
using KDBAbstractions.Repository.interfaces;
using MerchantService.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MerchantService.Test
{
    public class MerchantServiceTester
    {
        IKRepository<IMerchantModel> kRepo;
        [SetUp]
        public void Setup()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("kExpenseConfig.json", optional: false, reloadOnChange: false).Build();

            string connString = config["connectionString_test"].ToString();

            MerchantRepositoryCreator repoCreator = MerchantRepositoryCreator.Instanciate;
            var args = new MerchantRepositoryArgs();
            args.RepositoryType = config["repositoryType"];
            args.ConnectionString = connString;
            kRepo = repoCreator.Create(args);
        }


        [Test]
        public void TestGettingAllMerchants()
        {
            IMerchantService face = new ServiceFacade(kRepo);
            List<IMerchantModel> merchants = face.GetAll();
            Assert.Greater(merchants.Count, 0);
        }

        [Test]
        public void TestAddingMerchant()
        {
            IMerchantService face = new ServiceFacade(kRepo);

            List<IMerchantModel> merchants = face.GetAll();
            int firstCount = merchants.Count;
            IMerchantModel newMerchant = new MerchantModel { Name = $@"herman testing service''s {firstCount}", Address = "77 test lane , Brampton ON L6Y 1LT", Phone = "437-123-1234" };
            IMerchantModel addedMerchant = face.AddMerchant(newMerchant);
            merchants = face.GetAll();
            int secondCount = merchants.Count;

            Assert.NotZero(addedMerchant.Id);
            Assert.Greater(secondCount, firstCount);

        }


        [Test]
        public void TestDeletingMerchantJustAdded()
        {
            IMerchantService face = new ServiceFacade(kRepo);

            List<IMerchantModel> merchants = face.GetAll();
            int firstCount = merchants.Count;
            IMerchantModel newMerchant = new MerchantModel { Name = $@"herman testing service''s {firstCount}", Address = "77 test lane , Brampton ON L6Y 1LT", Phone = "437-123-1234" };
            IMerchantModel addedRecord = face.AddMerchant(newMerchant);
            merchants = face.GetAll();

            int secondCount = merchants.Count;
            Assert.Greater(secondCount, firstCount);
            face.DeleteMerchant(addedRecord);

            merchants = face.GetAll();
            int thirdCount = merchants.Count;
            Assert.AreEqual(firstCount, thirdCount);
        }


        [Test]
        public void TestUpdatingAMerchant()
        {

            IMerchantService face = new ServiceFacade(kRepo);

            List<IMerchantModel> merchants = face.GetAll();
            int firstCount = merchants.Count;
            IMerchantModel newMerchant = new MerchantModel { Name = $@"herman testing service''s {firstCount}", Address = "77 test lane , Brampton ON L6Y 1LT", Phone = "437-123-1234" };
            IMerchantModel addedRecord = face.AddMerchant(newMerchant);
    

            face.UpdateMerchant(new MerchantModel { Id = addedRecord.Id, Name = $"updated record {firstCount}", Phone = "416-123-3549" });

            IMerchantModel updatedRecord = face.GetById(addedRecord.Id);

            Assert.IsNotNull(updatedRecord);
            Assert.AreNotEqual(updatedRecord, addedRecord);
            Assert.AreNotEqual(updatedRecord.Phone, addedRecord.Phone);
            //int secondCount = merchants.Count;
        }


        [Test]
        public void TestDeletingMerchantThatHasExpenses()
        {
            try
            {
                IMerchantService face = new ServiceFacade(kRepo);

                List<IMerchantModel> merchants = face.GetAll();
                int firstCount = merchants.Count;
                IMerchantModel earlyMerchantThatIsProbablyUsed = merchants[0];

                face.DeleteMerchant(earlyMerchantThatIsProbablyUsed);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Expenses are related to this. wont delete", ex.Message);
            }

        }





    }
}
