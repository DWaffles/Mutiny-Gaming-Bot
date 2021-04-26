using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using Humanizer.Localisation;
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

            string joinDuration = (DateTime.Now - discordMember.JoinedAt).Humanize(3, maxUnit: TimeUnit.Year, minUnit: TimeUnit.Minute);
            string description = $"{discordMember.Mention} joined {ctx.Guild.Name} {joinDuration} ago.";

            var embed = new DiscordEmbedBuilder()
                    .WithAuthor($"{discordMember.DisplayName} User Information", iconUrl: $"{discordMember.AvatarUrl}")
                    .WithDescription(description)
                    .AddField("Created", dateTimeCreated, true)
                    .AddField("Joined", dateTimeJoined, true)
                    .WithFooter("Times are UTC using 24 hour time. AM/PM modifier for Americans.")
                    .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

            if (discordMember.Roles.Any())
            {
                var orderedRoles = discordMember.Roles.ToList().OrderByDescending(x => x.Position);
                var currentRoles = String.Join(", ", orderedRoles.Select(x => x.Mention));
                embed.AddField($"Current Roles ({discordMember.Roles.Count()})", currentRoles);
            }

            var memberEntity = await MemberService.GetOrCreateMemberAsync(discordMember);
            var pastRoles = memberEntity.RoleDictionary.Where(x => x.Value == false).Select(x => "<@&" + x.Key + ">"); //All Past Roles
            if (pastRoles.Any())
            {
                embed.AddField($"Previous Roles ({pastRoles.Count()})", String.Join(", ", pastRoles));
            }
            await ctx.RespondAsync(embed: embed);
        }
        [Command("user")]
        public async Task UserInformation(CommandContext ctx, [RemainingText] string memberName)
        {
            var members = await ctx.Guild.GetAllMembersAsync();
            var foundMember = FindMemberByName(members, memberName);
            if (foundMember != null)
            {
                await UserInformation(ctx, foundMember);
            }
            else
            {
                await ctx.RespondAsync(embed: MemberNotFoundEmbed());
            }
        }
        [Command("profile"), Aliases("pfp")]
        public async Task UserProfile(CommandContext ctx, DiscordMember discordMember = null)
        {
            await ctx.TriggerTypingAsync();

            if (discordMember == null)
            {
                discordMember = ctx.Member;
            }

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(MutinyBot.Config.HexCode))
                .WithImageUrl(discordMember.AvatarUrl);

            await ctx.RespondAsync(embed: embed);
        }
        [Command("profile")]
        public async Task UserProfile(CommandContext ctx, [RemainingText] string memberName)
        {
            var members = await ctx.Guild.GetAllMembersAsync();
            var foundMember = FindMemberByName(members.ToList(), memberName);
            if (foundMember != null)
            {
                await UserProfile(ctx, foundMember);
            }
            else
            {
                await ctx.RespondAsync(embed: MemberNotFoundEmbed());
            }
        }
    }
}