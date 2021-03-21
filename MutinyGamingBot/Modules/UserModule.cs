using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireGuild]
    public class UserModule : MutinyBotModule
    {
        [Command("user"), Aliases("u")]
        public async Task UserInformation(CommandContext ctx, DiscordMember discordMember = null)
        {
            await ctx.TriggerTypingAsync();

            if (discordMember == null)
            {
                discordMember = ctx.Member;
            }

            string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";
            string dateTimeJoined = discordMember.JoinedAt.ToUniversalTime().ToString(dateTimeFormat);
            string dateTimeCreated = discordMember.CreationTimestamp.ToUniversalTime().ToString(dateTimeFormat);

            string joinDuration;
            TimeSpan joinTimeSpan = DateTime.Now - discordMember.JoinedAt;
            if (joinTimeSpan.Days > 0)
                joinDuration = $"{joinTimeSpan.Days} days ago";
            else if (joinTimeSpan.Hours > 1)
                joinDuration = $"{joinTimeSpan.Hours} hours ago";
            else
                joinDuration = $"{joinTimeSpan.Minutes} minutes ago";

            string description = $"{discordMember.Mention} joined {ctx.Guild.Name} {joinDuration}.";

            var embed = new DiscordEmbedBuilder()
                    .WithAuthor($"{discordMember.DisplayName} User Information", iconUrl: $"{discordMember.AvatarUrl}")
                    .WithDescription(description)
                    .AddField("Created", dateTimeCreated, true)
                    .AddField("Joined", dateTimeJoined, true)
                    .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                    .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

            if (discordMember.Roles.Count() > 0)
            {
                var orderedRoles = discordMember.Roles.ToList().OrderByDescending(x => x.Position);
                var currentRoles = String.Join(", ", orderedRoles.Select(x => x.Mention));
                embed.AddField($"Current Roles ({discordMember.Roles.Count()})", currentRoles);
            }

            var memberEntity = await MemberService.GetOrCreateMemberAsync(discordMember);
            var pastRoles = memberEntity.RoleDictionary.Where(x => x.Value == false).Select(x => "<@&" + x.Key + ">"); //All Past Roles
            if (pastRoles.Count() > 0)
            {
                embed.AddField($"Previous Roles ({pastRoles.Count()})", String.Join(", ", pastRoles));
            }
            await ctx.RespondAsync(embed: embed);
        }
        [Command("user")]
        public async Task UserInformation(CommandContext ctx, [RemainingText] string memberName)
        {
            var members = ctx.Guild.Members.Values;
            if (members.Count() != ctx.Guild.MemberCount)
                members = await ctx.Guild.GetAllMembersAsync();

            var foundMember = members.FirstOrDefault(member => member.DisplayName.Equals(memberName, StringComparison.OrdinalIgnoreCase));
            if (foundMember != null)
            {
                await UserInformation(ctx, foundMember);
            }
            else
            {
                foundMember = members.FirstOrDefault(member => member.Username.Equals(memberName, StringComparison.OrdinalIgnoreCase));
                if (foundMember != null)
                {
                    await UserInformation(ctx, foundMember);
                }
                else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "User Not Found",
                        Description = $"No such user with that nickname or username was found.",
                        Color = new DiscordColor(0xFF0000) // red
                    };

                    await ctx.RespondAsync(embed: embed);
                }
            }
        }
    }
}