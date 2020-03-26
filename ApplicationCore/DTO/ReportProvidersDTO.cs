
namespace ApplicationCore.DTO
{
  public  class ReportProvidersDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
        public int CountOrder { get; set; }
    }
}
