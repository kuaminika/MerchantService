using KDBAbstractions.Repository.interfaces;
using System;
using System.Collections.Generic;
using KDBAbstractions;
using MerchantService.Models;
using KDBAbstractions.Repository;

namespace MerchantService
{
    public class MerchantRepository : IKRepository<IMerchantModel>
    {
        private AKDBAbstraction dbAbstraction;

        public MerchantRepository(AKDBAbstraction dbAbstraction)
        {
            this.dbAbstraction = dbAbstraction;
        }

        /// <summary>
        /// it will delete a record based on the name or Id. 
        /// it will return ammount of rows affected;
        /// </summary>
        /// <param name="victim"></param>
        /// <returns></returns>
        public int DeleteRecord(IMerchantModel victim)
        {
            bool hasNoId = victim.Id < 1;

            if (hasNoId)
            {
                IMerchantModel victim1 =  findByName(victim);
                // TODO: need to make exceptions messages not harcoded. 
                if (victim1 == null)                
                    throw new Exception($"Cannot find merchant with name {victim.Name}");
                int result = DeleteRecordById(victim1);
                return result;
            }

            int result1 = DeleteRecordById(victim);
            return result1;

        }

        /// <summary>
        /// It will delete a record by id
        /// it will return amount of rows affected
        /// </summary>
        /// <param name="victim"></param>
        /// <returns></returns>
        public int DeleteRecordById(IMerchantModel victim)
        {
            checkIfHasExpenes(victim);
            MerchantDataModel physicalVictim = GetById(victim.Id) as MerchantDataModel;
            string query = $"DELETE FROM kForeignPartyOrgn where id={physicalVictim.Id} ";
            string query2 = $"DELETE FROM kOrgnDesc where id = {physicalVictim.DescId}";
            int rowsAffected = 0;
            KWriteResult outcome =  dbAbstraction.ExecuteWriteTransaction(query);
            rowsAffected+= outcome.AffectedRowCount;
            outcome = dbAbstraction.ExecuteWriteTransaction(query2);
            rowsAffected += outcome.AffectedRowCount;

            return rowsAffected;
        }

        private void checkIfHasExpenes(IMerchantModel victim)
        {
            string query = $"SELECT t.kThirdPartyOrgn_id from kExpense t where t.kThirdPartyOrgn_id={victim.Id};";
            IMerchantModel foundWithExpense = null;
            dbAbstraction.ExecuteReadTransaction(query, new AllMapper(kdt =>
            {
                if (!kdt.Read() || !kdt.YieldedResults) return;

                foundWithExpense = new MerchantLazyModel(this);
                foundWithExpense.Id = kdt.GetInt("kThirdPartyOrgn_id");
            }));

            if (foundWithExpense != null)
                throw new Exception("Expenses are related to this. wont delete");
        }

        public List<IMerchantModel> GetAll(int org_id = 0, string sortby = "id")
        {
            List<IMerchantModel> result = new List<IMerchantModel>();

            string query = $@"SELECT * 
                                from kForeignPartyOrgn m 
                          inner join kOrgnDesc d on m.`desc.id`= d.id
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
                    m.Phone = kdt.GetString("phone");
                    m.Address = kdt.GetString("address");
                    
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
                    string query = $@"INSERT INTO kOrgnDesc(`name`,`email`,`phone`,`address`) 
                                    value ( '{newRecord.Name}','{newRecord.Name}','{newRecord.Phone}','{newRecord.Address}');";
                    System.Diagnostics.Debug.WriteLine(query);
                    result = new MerchantDataModel();
                    result.Name = newRecord.Name;
                    // will add if not already existant
                    KWriteResult insertOutcome = dbAbstraction.ExecuteWriteTransaction(query);

                    result.DescId = (int)insertOutcome.LastInsertedId;
                }


                //TODO the KDBAbstraction librabrary needs to be modified to protect against SQL injection attacks .. as of now, queries are vulnerable to single quote
                //     then the queries should be rewritten  KSP_Param nameParam = new KSP_Param { Type = KSP_ParamType.Str };
                string finalInsertQuery = $@"insert into kForeignPartyOrgn ( name_denormed, email_denormed,`desc.id`) 
                                              values ('{result.Name}','{result.Name}',{result.DescId});";

                KWriteResult finalInsertOutocme = dbAbstraction.ExecuteWriteTransaction(finalInsertQuery);
                System.Diagnostics.Debug.WriteLine(finalInsertOutocme);

                result.Id = (int)finalInsertOutocme.LastInsertedId;


                return result;
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
          IMerchantModel found;
          var result = GetAll(first.Id);       
             
          found = result == null ? findByName(first): result[0];
          found.Name = string.IsNullOrEmpty(first.Name) ? found.Name : first.Name;
          found.Phone = string.IsNullOrEmpty(first.Phone) ? found.Phone : first.Phone;
          found.Address = string.IsNullOrEmpty(first.Address) ? found.Address : first.Address;

            string updateQuery =   $@"update kOrgnDesc set  `name` = '{found.Name}', phone = '{found.Phone}' , address ='{found.Address}' 
              where id = {((MerchantDataModel)found).DescId}";
            string updateQuery2 = $@"update kForeignPartyOrgn set name_denormed='{found.Name}' where id={found.Id}; ";
            KWriteResult finalUpdateOutocme = dbAbstraction.ExecuteWriteTransaction(updateQuery);
            int rowsAffected = (int)finalUpdateOutocme.AffectedRowCount;
             finalUpdateOutocme = dbAbstraction.ExecuteWriteTransaction(updateQuery2);

             rowsAffected+= finalUpdateOutocme.AffectedRowCount;
          return rowsAffected;
        }
    }
}

