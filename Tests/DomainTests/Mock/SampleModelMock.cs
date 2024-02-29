using System.ComponentModel.DataAnnotations;

namespace DomainTests.Mock
{
    public class SampleModelMock
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? ReferencedSampleModelMockId { get; set; }
        public ReferencedSampleModelMock? ReferencedSampleModelMock { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is SampleModelMock mock &&
                   Id == mock.Id &&
                   Name == mock.Name &&
                   ReferencedSampleModelMockId == mock.ReferencedSampleModelMockId &&
                   (ReferencedSampleModelMock?.Equals(mock.ReferencedSampleModelMock) ?? ReferencedSampleModelMock is null && mock.ReferencedSampleModelMock is null);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, ReferencedSampleModelMockId, ReferencedSampleModelMock);
        }
    }
}
