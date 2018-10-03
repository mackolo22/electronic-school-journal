using System.Text;

namespace ApplicationCore.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string input)
        {
            string asciiEquivalents = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(input));
            return asciiEquivalents;
        }
    }
}
