using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireGuild]
    public class GuildModule : MutinyBotModule
    {
        [Command("guild"), Aliases("g")]
        public async Task GuildInformation(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";
            string dateTimeCreated = ctx.Guild.CreationTimestamp.ToUniversalTime().ToString(dateTimeFormat);

            string joinDuration;
            TimeSpan joinTimeSpan = DateTime.Now - ctx.Guild.CreationTimestamp;
            if (joinTimeSpan.Days > 0)
                joinDuration = $"{joinTimeSpan.Days} days ago";
            else if (joinTimeSpan.Hours > 1)
                joinDuration = $"{joinTimeSpan.Hours} hours ago";
            else
                joinDuration = $"{joinTimeSpan.Minutes} minutes ago";

            string description = $"{ctx.Guild.Name} was created {joinDuration}.";

            var embed = new DiscordEmbedBuilder()
                    .WithAuthor($"{ctx.Guild.Name} Information", iconUrl: $"{ctx.Guild.IconUrl}")
                    .WithDescription(description)
                    .AddField("Created", dateTimeCreated, true)
                    .AddField("Members", ctx.Guild.MemberCount.ToString(), true)
                    .AddField("Roles", ctx.Guild.Roles.Count.ToString(), true)
                    .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                    .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

            await ctx.RespondAsync(embed: embed);
        }
    }
}
