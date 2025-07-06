using System.Text.RegularExpressions;

namespace Utilities
{
    public static class TextHighlighter
    {
        public static string EmphasizeTerm(string text, string term)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(term))
                return text;

            var pattern = Regex.Escape(term);
            return Regex.Replace(text, pattern, m => $"<<{m.Value}>>", RegexOptions.IgnoreCase);
        }
    }
}
