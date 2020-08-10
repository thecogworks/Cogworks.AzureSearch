using Cogworks.AzureSearch.Constants.StringConstants;

namespace Cogworks.AzureSearch.Extensions
{
    internal static class StringExtensions
    {
        public static bool HasValue(this string input)
            => !string.IsNullOrWhiteSpace(input);

        public static string EscapeHyphen(this string input)
            => input.Escape(Separators.Hyphen);

        public static string Escape(this string input, string character)
            => input.HasValue()
                ? input.Replace(
                    $"{character}",
                    $@"\{character}")
                : input;
    }
}