using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantService.Models
{
    internal class MerchantLazyModel : IMerchantModel
    {
        private MerchantDataModel _innerSelf;
        private MerchantRepository repo;
        private MerchantDataModel innerSelf { get {
                _innerSelf = _innerSelf ?? seekInnerSelf();
                return _innerSelf;            
            } }

        private MerchantDataModel seekInnerSelf()
        {
            IMerchantModel merchantData = repo.GetById(this.Id);
            return (MerchantDataModel)merchantData;
        }

        internal MerchantLazyModel(MerchantRepository repo)
        {       
            this.repo = repo;
        }

        public int Id   {   get; set;  }
        public string Name {
            get
            {
                string name = innerSelf.Name;
                return name;            
            } 
            set 
            {
                innerSelf.Name = value;
                repo.UpdateRecord(innerSelf);            
            } }
        public string Phone
        {
            get
            {
                string phone = innerSelf.Phone;
                return phone;
            }
            set
            {
                innerSelf.Phone = value;
                repo.UpdateRecord(innerSelf);
            }
        }
        public string Address
        {
            get
            {
                string address = innerSelf.Address;
                return address;
            }
            set
            {
                innerSelf.Address = value;
                repo.UpdateRecord(innerSelf);
            }
        }
    }
}
