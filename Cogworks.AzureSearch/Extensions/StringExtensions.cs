namespace Cogworks.AzureSearch.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string input)
            => !string.IsNullOrWhiteSpace(input);

        public static string EscapeHyphen(this string input)
            => input.Escape("-");

        public static string Escape(this string input, string character)
            => input.HasValue()
                ? input.Replace(
                    $"{character}",
                    $@"\{character}")
                : input;
    }
}