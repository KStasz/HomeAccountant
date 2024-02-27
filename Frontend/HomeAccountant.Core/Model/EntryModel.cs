namespace HomeAccountant.Core.Model
{
    public class EntryModel : IClearableObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel? Category { get; set; }
        public BillingPeriodModel? Period { get; set; }
        public decimal Price { get; set; }
        public string? Creator { get; set; }
        public DateTime CreatedDate { get; set; }

        public void Clear()
        {
            Name = null;
            Category = null;
            Period = null;
            Price = 0;
            Creator = null;
            CreatedDate = DateTime.Now;
        }
    }
}
