using HomeAccountant.Core.Model;
using System.Diagnostics.CodeAnalysis;

namespace HomeAccountant.CoreTests.Comparers
{
    public class CategoryModelCollectionComparer : IEqualityComparer<IEnumerable<CategoryModel>>
    {
        public bool Equals(IEnumerable<CategoryModel>? x, IEnumerable<CategoryModel>? y)
        {
            if (x is null && y is null) 
                return true;

            if (x is null || y is null)
                return false;

            foreach (var item in x)
            {
                if (!y.Contains(item))
                    return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] IEnumerable<CategoryModel> obj)
        {
            if (obj is null)
            {
                return 0;
            }
            
            var concatedObjects = obj.Select(x => string.Concat(x.Id, x.Name));
            var concatedString = string.Empty;
            
            foreach (var item in concatedObjects)
            {
                string.Concat(concatedString, item);
            }

            return concatedString.GetHashCode();
        }
    }
}
