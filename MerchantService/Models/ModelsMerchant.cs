namespace MerchantService.Models
{
    public class MerchantModel : IMerchantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}