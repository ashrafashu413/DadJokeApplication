

namespace Utilities
{
    public static class JokeLengthClassifier
    {
        public static JokeLengthGroup Classify(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return JokeLengthGroup.Short;

            var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            return wordCount switch
            {
                < 10 => JokeLengthGroup.Short,
                < 20 => JokeLengthGroup.Medium,
                _ => JokeLengthGroup.Long
            };
        }
    }
}
