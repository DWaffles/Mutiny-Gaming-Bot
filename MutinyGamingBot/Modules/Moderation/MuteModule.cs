using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using Humanizer.Localisation;
using MutinyBot.Enums;
using MutinyBot.Extensions;
using MutinyBot.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("mute"), Aliases("m"), RequireGuild]
    [RequireBotPermissions(Permissions.ManageRoles), RequireUserPermissions(Permissions.ManageRoles)]
    [Description("Commands relating to muting people. Requires manage roles permissions.")]
    public class MuteModule : MutinyBotModule
    {
        [GroupCommand()]
        [Description("Applies the servers mute role to the member for 24 hours.")]
        public async Task BaseMuteCommand(CommandContext ctx,
            [Description("Mention the member to be muted.")] DiscordMember memberToMute,
            [Description("Given reason for muting the member."), RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            await MuteCommandHandler(ctx, memberToMute, TimeSpan.FromHours(24), reason);
        }
        [Command("permanent"), Aliases("perm", "p")]
        [Description("Applies the servers mute role to the member for an indefinite period of time.")]
        public async Task PermanentMuteCommand(CommandContext ctx,
            [Description("Mention the member to be muted.")] DiscordMember memberToMute,
            [Description("Given reason for muting the member."), RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            await MuteCommandHandler(ctx, memberToMute, null, reason);
        }
        [Command("temporary"), Aliases("temp", "t")]
        [Description("Applies the servers mute role to the member for the given period of time.")]
        public async Task TemporaryMuteCommand(CommandContext ctx,
            [Description("Length of time to mute the member for. Supports h for hours, d for days, m for minutes, s for seconds. " +
            "Ex: `5h` -> 5 hours; `2d5h` for 2 days & 5 hours; `6h30m` -> for 6 hours 30 minutes.")] TimeSpan muteDuration,
            [Description("Mention the member to be muted.")] DiscordMember memberToMute,
            [Description("Given reason for muting the member."), RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            await MuteCommandHandler(ctx, memberToMute, muteDuration, reason);
        }
        [Command("list"), Aliases("l")]
        [Description("Gets the list of current mutes.")]
        public async Task ListActiveMutesCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
        }

        #region HelperFunctions
        private async Task<(bool ChecksPassed, DiscordRole MuteRole, MemberModel MemberModel, string ErrorMessage)> RunCommandChecksAsync(DiscordGuild guild, DiscordMember memberToMute, TimeSpan timeForMute)
        {
            var memberModel = await MemberService.GetOrCreateMemberAsync(guild.Id, memberToMute.Id);
            var guildModel = memberModel.Guild;

            if (guildModel.MuteRoleId == 0)
            {
                return (false, null, memberModel, $"{guild.Name} does not have a set mute role.");
            }

            var muteRole = guild.GetRole(guildModel.MuteRoleId);
            if (muteRole is null)
            {
                return (false, null, memberModel, $"I am unable to retrieve {guild.Name}'s set mute role.");
            }
            else if (muteRole.Position >= guild.CurrentMember.Roles.Max(role => role.Position))
            {
                return (false, muteRole, memberModel, $"`@{muteRole.Name}` is higher or the same as my highest role.");
            }
            else if(memberToMute.Roles.Contains(muteRole))
            {
                return (false, muteRole, memberModel, $"**{memberToMute.Username}#{memberToMute.Discriminator}** already has the set mute role of `@{muteRole.Name}`.");
            }
            else if (timeForMute.CompareTo(TimeSpan.Zero) < 0)
            {
                return (false, muteRole, memberModel, $"The given time is negative or otherwise invalid.");
            }
            else
            {
                return (true, muteRole, memberModel, String.Empty);
            }
        }
        private async Task MuteCommandHandler(CommandContext ctx, DiscordMember memberToMute, TimeSpan? timeForMute, string reason = null)
        {
            var checkResult = await RunCommandChecksAsync(ctx.Guild, memberToMute, TimeSpan.Zero);
            if (!checkResult.ChecksPassed)
            {
                await ctx.RespondAsync(checkResult.ErrorMessage);
                return;
            }

            var embed = GetMuteConfirmationEmbed(ctx.Guild, ctx.Member, memberToMute, checkResult.MemberModel, timeForMute, reason);
            var (userResponse, interaction) = await ctx.WaitForConfirmationInteraction(embed);

            if (userResponse is ConfirmationResult.TimedOut || userResponse is ConfirmationResult.Denied)
            {
                await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                return;
            }
            else
            {
                await interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                await memberToMute.GrantRoleAsync(checkResult.MuteRole, $"Action initated by {ctx.Member.Username}#{ctx.Member.Discriminator}: {reason ?? "No reason given."}");
                checkResult.MemberModel.TimesMuted++;
                await MemberService.UpdateMemberAsync(checkResult.MemberModel);

                var warned = true;
                try // DM member
                {
                    var dmChannel = await memberToMute.CreateDmChannelAsync();
                    await dmChannel.SendMessageAsync("warn");
                }
                catch
                {
                    warned = false;
                }

                var moderationChannel = ctx.Guild.GetChannel(checkResult.MemberModel.Guild.ModerationLogChannelId);
                if (moderationChannel != null)
                {
                    await moderationChannel.SendMessageAsync($"Test. {(warned ? "warned" : "not warned.")}");
                    //send moderation message
                }
                await interaction.EditOriginalResponseAsync("Muted");
            }
        }
        private DiscordEmbed GetMuteConfirmationEmbed(DiscordGuild guild, DiscordMember initatingMember, DiscordMember memberToMute, MemberModel memberToMuteModel, TimeSpan? timeForMute, string reason)
        {
            return new DiscordEmbedBuilder()
                .WithTitle($"Mute {memberToMute.Username}#{memberToMute.Discriminator}?")
                .WithThumbnail(memberToMute.AvatarUrl)
                .AddField("Member", memberToMute.Mention, true)
                .AddField("Length of Mute", timeForMute?.Humanize(precision: 3, maxUnit: TimeUnit.Week, minUnit: TimeUnit.Second) ?? $"Indefinite.", true)
                .AddField("Times Muted Before", memberToMuteModel.TimesMuted.ToString(), true)
                .AddField("Reason", reason ?? $"None given by {initatingMember.Mention}.", false)
                .WithTimestamp(DateTime.Now)
                .WithFooter($"Member ID: {memberToMute.Id}")
                .WithColor(GetBotColor());
        }
        //get user was muted
        #endregion
    }
}
