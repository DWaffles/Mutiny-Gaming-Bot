using System;

namespace MutinyBot.Common
{
    public static class UtilityHelpers
    {
        public static string SanitizeString(string givenString)
        {
            if (givenString.Contains("\""))
            {
                givenString = givenString.Replace("\"", "\"\"");
            }
            if (givenString.Contains(","))
            {
                givenString = String.Format("\"{0}\"", givenString);
            }
            return givenString;
        }
    }
}
