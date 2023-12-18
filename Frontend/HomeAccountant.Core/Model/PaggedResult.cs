using System.Text.Json.Serialization;

namespace HomeAccountant.Core.Model
{
    public class PaggedResult<T>
    {
        [JsonConstructor]
        public PaggedResult()
        {

        }

        public PaggedResult(IEnumerable<T> collection, int page, int totalPages)
        {
            Result = collection;
            CurrentPage = page;
            TotalPages = totalPages;
        }

        public IEnumerable<T> Result { get;  set; }
        public int CurrentPage { get;  set; }
        public int TotalPages { get;  set; }
    }
}
