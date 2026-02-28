using System.Globalization;
using System.Text;

namespace FastFoodAPI.Helpers
{
    public static class StringHelper
    {
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicode = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicode != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd')
                .Replace('Đ', 'D')
                .ToLower();
        }
    }
}