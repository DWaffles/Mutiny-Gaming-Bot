using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using MutinyBot.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules.Moderation
{
    [Description("Commands relating to muting people.")]
    [RequireGuild]
    public class MuteModule : MutinyBotModule
    {
        /*[Command("mute")]
        [Description("Users with permissions are able to use this command to apply the set mute role to people for 24 hours or a set duration.")]
        public async Task MuteUser(CommandContext ctx,
            [Description("[Required] Mention the member to be muted.")] DiscordMember member,
            [Description("[Optional] Given reason for muting the member."), RemainingText] string reason = null)
        {
            await MuteUser(ctx, member, new TimeSpan(hours: 24, minutes: 0, seconds: 0), reason);
        }
        [Command("mute")]
        public async Task MuteUser(CommandContext ctx,
            [Description("[Required] Mention the member to be muted.")] DiscordMember member,
            [Description("[Required] The time period to mute the user. Only supports h for hours, and d for days." +
            "\nEx, \"5h\" -> 5 hours, or \"2d\" for 2 days.")] TimeSpan muteTime,
            [Description("[Optional] Given reason for muting the member."), RemainingText] string reason = null)
        {}*/
        [Command("pmute"), Aliases("permanentmute")]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [RequireUserPermissions(Permissions.ManageRoles)]
        [Description("Users with permissions are able to use this command to apply the set mute role to people for an indefinite period of time.")]
        public async Task PermanmentMuteUser(CommandContext ctx,
            [Description("[Required] Mention the member to be muted.")] DiscordMember mentionedMember,
            [Description("[Optional] Given reason for muting the member."), RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();

            var guildConfig = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);

            if (guildConfig.MuteRoleId == 0)
            {
                await ctx.RespondAsync($"{ctx.Guild.Name} does not have a set mute role.");
                return;
            }

            DiscordRole muteRole = ctx.Guild.GetRole(guildConfig.MuteRoleId);
            if (muteRole is null)
            {
                await ctx.RespondAsync($"I am unable to retrieve {ctx.Guild.Name}'s set mute role.");
                return;
            }

            if (muteRole.Position >= ctx.Guild.CurrentMember.Roles.Max(role => role.Position))
            {
                await ctx.RespondAsync($"{muteRole.Name} is higher or the same as my highest role.");
                return;
            }

            await mentionedMember.GrantRoleAsync(muteRole, String.Join(": ", "Action by " + ctx.Member.DisplayName, reason ?? "No reason given."));
            var memberEntity = await MemberService.GetOrCreateMemberAsync(mentionedMember);
            memberEntity.TimesMuted++;
            await MemberService.UpdateMemberAsync(memberEntity);
            bool dmEnabled = false;
            try
            {
                string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";
                string description = $"You have been indefinitely muted in {Formatter.Bold(ctx.Guild.Name)} by {ctx.User.Mention}."
                    + $"\n\nThis your {memberEntity.TimesMuted.ToOrdinalWords()} warning in {ctx.Guild.Name}.";

                var embed = new DiscordEmbedBuilder()
                        .WithAuthor($"Mute given in: {ctx.Guild.Name}", iconUrl: $"{ctx.Guild.IconUrl}")
                        .WithDescription(description)
                        .AddField("Reason", (reason is not null ? reason : $"None given by {ctx.User.Mention}."), false)
                        .AddField("Time", ctx.Message.Timestamp.UtcDateTime.ToString(dateTimeFormat), true)
                        .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                        .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

                if (ctx.Channel.PermissionsFor(mentionedMember).HasPermission(Permissions.AccessChannels))
                    embed.AddField("Context", Formatter.MaskedUrl("Message", ctx.Message.JumpLink), true);

                _ = await mentionedMember.SendMessageAsync(embed: embed);
                dmEnabled = true;
            }
            catch { }

            DiscordChannel modChannel = ctx.Guild.GetChannel(guildConfig.ModerationLogChannelId);
            if (modChannel is not null)
            {
                string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";
                string description = $"{mentionedMember.Mention} has been muted by {ctx.User.Mention} for an indefinite period of time."
                    + $"\n\nThis is {mentionedMember.Mention}'s {memberEntity.TimesMuted.ToOrdinalWords()} warning in {ctx.Guild.Name}.";

                if (dmEnabled)
                {
                    description += $"\n\n{mentionedMember.Mention} was automatically messaged about this action.";
                }
                else
                {
                    description += $"\n\n{mentionedMember.Mention} does not have their DM's enabled and {Formatter.Bold("was not")} automatically messaged.";
                }

                var embed = new DiscordEmbedBuilder()
                        .WithAuthor($"Member Muted: {mentionedMember.DisplayName} by {ctx.Member.DisplayName}", iconUrl: $"{ctx.Guild.IconUrl}")
                        .WithDescription(description)
                        .AddField("Reason", (reason is not null ? reason : $"None given by {ctx.User.Mention}."), false)
                        .AddField("Time", ctx.Message.Timestamp.UtcDateTime.ToString(dateTimeFormat), true)
                        .AddField("Context", Formatter.MaskedUrl("Message", ctx.Message.JumpLink), true)
                        .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                        .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

                await modChannel.SendMessageAsync(embed: embed);
            }
            if (dmEnabled)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                await ctx.RespondAsync(emoji);
            }
            else
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":yellow_square:");
                await ctx.RespondAsync($"{emoji} Action completed, although {mentionedMember.Mention} has disabled DM's.");
            }
        }
        [Command("setmute")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task JoinLogSetChannel(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            GuildEntity guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);

            guildEntity.MuteRoleId = role.Id;
            await GuildService.UpdateGuildAsync(guildEntity);

            await ctx.RespondAsync($"{role.Name} is the new mute role for {Formatter.Bold(ctx.Guild.Name)}.");
        }
        //unmute
    }
}
