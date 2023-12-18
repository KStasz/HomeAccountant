using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PaggedResult<T>
    {
        public PaggedResult(IEnumerable<T> result, int currentPage, int totalPages)
        {
            Result = result;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public IEnumerable<T> Result { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
    }
}
