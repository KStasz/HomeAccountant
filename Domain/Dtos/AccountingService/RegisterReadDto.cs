namespace Domain.Dtos.AccountingService
{
    public class RegisterReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? CreatorId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
