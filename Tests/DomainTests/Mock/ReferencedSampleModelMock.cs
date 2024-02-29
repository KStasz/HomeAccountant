
namespace DomainTests.Mock
{
    public class ReferencedSampleModelMock
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int? SampleModelMockId { get; set; }
        public SampleModelMock? SampleModelMock { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReferencedSampleModelMock mock &&
                   Id == mock.Id &&
                   Name == mock.Name &&
                   Description == mock.Description &&
                   SampleModelMockId == mock.SampleModelMockId &&
                   (SampleModelMock?.Equals(mock.SampleModelMock) ?? SampleModelMock is null && mock.SampleModelMock is null);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, SampleModelMockId, SampleModelMock);
        }
    }
}
