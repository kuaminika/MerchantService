using KDBAbstractions.Repository.interfaces;
using System;
using System.Collections.Generic;
using KDBAbstractions;
using MerchantService.Models;

namespace MerchantService
{
    public class MerchantRepository : IKRepository<IMerchantModel>
    {
        private AKDBAbstraction dbAbstraction;

        public MerchantRepository(AKDBAbstraction dbAbstraction)
        {
            this.dbAbstraction = dbAbstraction;
        }
        public int DeleteRecord(IMerchantModel victim)
        {
            throw new NotImplementedException();
        }

        public int DeleteRecordById(IMerchantModel victim)
        {
            throw new NotImplementedException();
        }

        public List<IMerchantModel> GetAll(int org_id = 0, string sortby = "id")
        {
            List<IMerchantModel> result = new List<IMerchantModel>();

            string query = $@"SELECT * 
                                from kForeignPartyOrgn m 
                               where (m.id={org_id} and 0<>{org_id})
                                  or (m.id<>{org_id} and 0={org_id})
                               order by m.{sortby}";
            dbAbstraction.ExecuteReadTransaction(query, new AllMapper((kdt) =>
            {
                while (kdt.Read())
                {
                    var m = new MerchantDataModel();
                    m.Id = kdt.GetInt("Id");
                    m.Name = kdt.GetString("name_denormed");
                    m.DescId = kdt.GetInt("desc.id");
                    result.Add(m);
                }
            }));

            return result;
        }

        public IMerchantModel GetById(int id)
        {
            List<IMerchantModel> results = GetAll(id);
            if (results.Count == 0) return null;

            IMerchantModel result = results[0];
            return result;
        }




        public IMerchantModel Record(IMerchantModel newRecord)
        {
            try
            {
                IMerchantModel existing = findByName(newRecord);
                bool alreadyExists = existing != null && existing.Id>0;
                if (alreadyExists) return existing;

                MerchantDataModel result = new MerchantDataModel();
                result.Name = newRecord.Name;
                result = findPartyByName(result);

                if (result== null)
                {
                    string query = $"INSERT INTO kOrgnDesc(`name`,`email`) value ( '{newRecord.Name}','{newRecord.Name}');";
                    System.Diagnostics.Debug.WriteLine(query);
                    result = new MerchantDataModel();
                    result.Name = newRecord.Name;
                    // will add if not already existant
                    KWriteResult insertOutcome = dbAbstraction.ExecuteWriteTransaction(query);

                    result.DescId = (int)insertOutcome.LastInsertedId;
                }

                string finalInsertQuery = $@"insert into kForeignPartyOrgn ( name_denormed, email_denormed,`desc.id`) 
                                              values ('{result.Name}','{result.Name}',{result.DescId});";

                KWriteResult finalInsertOutocme = dbAbstraction.ExecuteWriteTransaction(finalInsertQuery);
                System.Diagnostics.Debug.WriteLine(finalInsertOutocme);

                result.Id = (int)finalInsertOutocme.LastInsertedId;


                return newRecord;
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// this will search by given newRecord.Name
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        private MerchantDataModel findPartyByName(MerchantDataModel newRecord)
        {
            try
            {
                string query = string.Format("SELECT id from kOrgnDesc p where p.name='{0}'", newRecord.Name);
                dbAbstraction.ExecuteReadTransaction(query, new AllMapper(kdataReader =>
                {
                    if (!kdataReader.Read() || !kdataReader.YieldedResults) return;
                    newRecord.DescId = kdataReader.GetInt("id");

                }));
            }
            catch (Exception)
            {
                throw;
            }

            if (newRecord.DescId < 1) return null;
            return newRecord;
        }


        /// <summary>
        /// this will search by given newRecord.Name
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        private IMerchantModel findByName(IMerchantModel newRecord)
        {
            try
            {
                string merchantSearchQuery = string.Format("SELECT id from kForeignPartyOrgn p where p.name_denormed='{0}'", newRecord.Name);
                dbAbstraction.ExecuteReadTransaction(merchantSearchQuery, new AllMapper(kdataReader =>
                {
                    if (!kdataReader.Read() || !kdataReader.YieldedResults) return;
                    newRecord.Id = kdataReader.GetInt("id");

                }));
            }
            catch (Exception)
            {
                throw;
            }

            if (newRecord.Id < 1) return null;
            return newRecord;
        }

        public int UpdateRecord(IMerchantModel first)
        {
            throw new NotImplementedException();
        }
    }
}

