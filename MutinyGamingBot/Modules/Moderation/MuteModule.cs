using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
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
            await ctx.RespondAsync($"Pong: {ctx.Client.Ping}ms");
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
    }
}
