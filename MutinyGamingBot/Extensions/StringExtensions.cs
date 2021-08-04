using System.Linq;

namespace MutinyBot.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a copy of this string with the first character capitalized.
        /// </summary>
        /// <returns>The string with the first character capitalized.</returns>
        public static string CapitalizeFirst(this string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
