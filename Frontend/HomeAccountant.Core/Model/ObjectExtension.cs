using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.Model
{
    public static class ObjectExtension
    {
        public static T Protect<T>(this T? value, [CallerMemberName] string propertyName = "")
        {
            return value ?? throw new ArgumentNullException(propertyName);
        }
    }
}
