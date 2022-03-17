namespace MerchantService.Models
{
    public interface IMerchantModel
    {
        int Id { get; set; }
        string Name { get; set; }
        string Phone { get; set; }
        string Address { get; set; }
    }
}
