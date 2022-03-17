namespace MerchantService.Models
{
    public class MerchantDataModel : IMerchantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DescId { get; set; }
        public string Phone { get;  set; }
        public string Address { get;  set; }
    }
}