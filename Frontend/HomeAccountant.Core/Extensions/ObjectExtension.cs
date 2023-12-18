using System.Runtime.CompilerServices;

namespace HomeAccountant.Core.Extensions
{
    public static class ObjectExtension
    {
        public static T Protect<T>(this T? value, [CallerMemberName] string propertyName = "")
        {
            return value ?? throw new ArgumentNullException(propertyName);
        }
    }
}
