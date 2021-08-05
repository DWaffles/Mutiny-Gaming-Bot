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
            await ctx.RespondAsync($"Pong: {ctx.Client.Ping}ms");
        }
        [Command("permanent"), Aliases("perm", "p")]
        [Description("Applies the servers mute role to the member for an indefinite period of time.")]
        public async Task PermanentMuteCommand(CommandContext ctx,
            [Description("Mention the member to be muted.")] DiscordMember memberToMute,
            [Description("Given reason for muting the member."), RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();

            var checkResult = await RunCommandChecksAsync(ctx.Guild, memberToMute);
            if(!checkResult.ChecksPassed)
            {
                await ctx.RespondAsync(checkResult.ErrorMessage);
                return;
            }

            var embed = GetMuteConfirmationEmbed(ctx.Guild, ctx.Member, memberToMute, checkResult.MemberModel, default, reason);
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

                var moderationChannel = ctx.Guild.GetChannel(checkResult.GuildModel.ModerationLogChannelId);
                if (checkResult.GuildModel.ModerationLogChannelId != 0 && moderationChannel != null)
                {
                    //send moderation message
                }

                await interaction.EditOriginalResponseAsync("Muted");
            }
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
            await ctx.RespondAsync($"{muteDuration.TotalDays} days, {muteDuration.TotalHours} hours, {muteDuration.TotalMinutes} minutes, {muteDuration.Seconds} seconds");
        }
        [Command("list"), Aliases("l")]
        [Description("Gets the list of current mutes.")]
        public async Task ListActiveMutesCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
        }

        #region HelperFunctions
        private async Task<(bool ChecksPassed, DiscordRole MuteRole, GuildModel GuildModel, MemberModel MemberModel, string ErrorMessage)> RunCommandChecksAsync(DiscordGuild guild, DiscordMember memberToMute)
        {
            var guildModel = await GuildService.GetOrCreateGuildAsync(guild.Id);
            var memberModel = await MemberService.GetOrCreateMemberAsync(guild.Id, memberToMute.Id);

            if (guildModel.MuteRoleId == 0)
            {
                return (false, null, guildModel, memberModel, $"{guild.Name} does not have a set mute role.");
            }

            var muteRole = guild.GetRole(guildModel.MuteRoleId);
            if (muteRole is null)
            {
                return (false, null, guildModel, memberModel, $"I am unable to retrieve {guild.Name}'s set mute role.");
            }
            else if (muteRole.Position >= guild.CurrentMember.Roles.Max(role => role.Position))
            {
                return (false, muteRole, guildModel, memberModel, $"`@{muteRole.Name}` is higher or the same as my highest role.");
            }
            else if(memberToMute.Roles.Contains(muteRole))
            {
                return (false, muteRole, guildModel, memberModel, $"**{memberToMute.Username}#{memberToMute.Discriminator}** already has the set mute role of `@{muteRole.Name}`.");
            }
            else
            {
                return (true, muteRole, guildModel, memberModel, String.Empty);
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
