
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

        public override bool Equals(object? obj)
        {
            return obj is EntryModel model &&
                   Id == model.Id &&
                   Name == model.Name &&
                   CategoryId == model.CategoryId &&
                   (Category is null ? new CategoryModel() : Category).Equals(model.Category is null ? new CategoryModel() : model.Category) &&
                   (Period is null ? new BillingPeriodModel() : Period).Equals(model.Period is null ? new BillingPeriodModel() : model.Period) &&
                   Price == model.Price &&
                   Creator == model.Creator &&
                   CreatedDate == model.CreatedDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, CategoryId, Category, Period, Price, Creator, CreatedDate);
        }
    }
}
