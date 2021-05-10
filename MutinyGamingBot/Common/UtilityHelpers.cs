using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MutinyBot.Common
{
    public static class UtilityHelpers
    {
        private static readonly Regex embedFileTypes = new(".(png|jp(e)?g|gif|webp)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
        public static string FirstValidEmbedMediaUrl(DiscordMessage message)
        {
            foreach (var attachment in message.Attachments)
            {
                if (embedFileTypes.Match(attachment.MediaType).Success)
                {
                    return attachment.MediaType;
                }
            }
            foreach (var embed in message.Embeds)
            {
                if (new List<string> { "image", "video" }.Contains(embed.Type, StringComparer.OrdinalIgnoreCase))
                {
                    if (embedFileTypes.Match(embed.Url.AbsoluteUri).Success)
                        return embed.Url.AbsoluteUri;
                }
            }
            return null;
        }
        public static IEnumerable<string> GetAttachmentMediaTypes(DiscordMessage message)
        {
            return message.Attachments.Select(x => x.MediaType);

            /*if(!testOne.Any())
            {
                mediaTypes = message.Embeds.Select(x => x.Type);
            }
            return mediaTypes;
            */
        }
    }
}
