namespace Domain.Dtos.AccountingService
{
    public class BillingPeriodReadDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public RegisterReadDto? Register { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
