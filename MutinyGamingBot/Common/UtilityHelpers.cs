using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static string FirstImageOrVideoUrl(DiscordMessage message)
        {
            foreach (var attachment in message.Attachments)
            {
                if (attachment.Height != 0 && attachment.Width != 0)
                {
                    return attachment.Url;
                }
            }
            foreach (var embed in message.Embeds)
            {
                if (new List<string> { "image", "video" }.Contains(embed.Type, StringComparer.OrdinalIgnoreCase))
                {
                    return embed.Url.ToString();
                }
            }
            return null;
        }
    }
}
