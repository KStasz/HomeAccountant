using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;

namespace HomeAccountant.Core.DTOs.Entry
{
    public record EntryReadDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public CategoryReadDto? Category { get; init; }
        public BillingPeriodReadDto? Period { get; init; }
        public decimal Price { get; init; }
        public string? Creator { get; init; }
        public DateTime CreatedDate { get; init; }

    }
}
