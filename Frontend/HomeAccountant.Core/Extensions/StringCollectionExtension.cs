namespace HomeAccountant.Core.Extensions
{
    public static class StringCollectionExtension
    {
        public static string JoinToString(this IEnumerable<string>? input, string? separator = null)
        {
            return string.Join(separator ?? Environment.NewLine, input ?? Array.Empty<string>());
        }
    }
}
