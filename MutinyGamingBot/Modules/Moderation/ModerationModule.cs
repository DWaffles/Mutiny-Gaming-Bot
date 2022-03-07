using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Enums;
using MutinyBot.Extensions;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("moderation"), Aliases("mod")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    [RequireGuild]
    [Description("Commands relating to managing the moderation settings of the guild. Requires ManageGuild permissions.")]
    public class ModerationModule : MutinyBotModule
    {
        [GroupCommand()]
        [Description("Lists the current set moderation settings of the guild.")]
        public async Task ModerationSettingsCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id, false);

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"Guild Settings For {ctx.Guild.Name}", iconUrl: ctx.Guild.IconUrl)
                .WithFooter($"Guild ID: {ctx.Guild.Id}")
                .WithColor(GetBotColor());

            if (guildEntity.TrackMemberRoles)
                embed.AddField($"Track Member Roles?", guildEntity.TrackMemberRoles.ToString(), true);
            if (guildEntity.TrackMessageTimestamps)
                embed.AddField($"Track Member Activity?", guildEntity.TrackMessageTimestamps.ToString(), true);
            if (guildEntity.MuteRoleId != 0)
            {
                if (ctx.Guild.GetRole(guildEntity.MuteRoleId) != null)
                    embed.AddField($"Mute Role", $"<@&{guildEntity.MuteRoleId}>", true);
                else
                    guildEntity.MuteRoleId = 0;
            }
            if (guildEntity.ModerationLogChannelId != 0)
            {
                if (ctx.Guild.GetChannel(guildEntity.ModerationLogChannelId) != null)
                    embed.AddField($"Moderation Log Channel", $"<#{guildEntity.ModerationLogChannelId}>", true);
                else
                    guildEntity.ModerationLogChannelId = 0;
            }
            if (guildEntity.JoinLogChannelId != 0)
            {
                if (ctx.Guild.GetChannel(guildEntity.JoinLogChannelId) != null)
                    embed.AddField($"Join Log Channel", $"<#{guildEntity.JoinLogChannelId}>", true);
                else
                    guildEntity.JoinLogChannelId = 0;
            }

            await GuildService.UpdateGuildAsync(guildEntity);
            await ctx.RespondAsync(embed: embed);
        }
        [Command("joinlog"), Aliases("join", "jl")]
        [Description("Allows setting and removal of the join log channel. Calling this when there is a channel set will ask for confirmation to remove it, " +
            "while passing a channel to this command will ask to update the join log channel.")]
        public async Task ChangeJoinLogChannelCommand(CommandContext ctx, [RemainingText] DiscordChannel newChannel = null)
        {
            await ctx.TriggerTypingAsync();

            var guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id, false);
            var currentChannel = ctx.Guild.GetChannel(guildEntity.JoinLogChannelId);

            if (newChannel == null && currentChannel != null) //disable joinlog
            {
                var (userResponse, interaction) = await ctx.WaitForConfirmationInteraction($"Remove set join log for **{ctx.Guild.Name}**? Current join log channel: {currentChannel.Mention}.");

                if (userResponse is ConfirmationResult.Confirmed)
                {
                    guildEntity.JoinLogChannelId = 0;
                    await GuildService.UpdateGuildAsync(guildEntity);

                    await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, $":white_check_mark: Disabled joinlog for **{ctx.Guild.Name}**.");
                }
                else
                    await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            }
            else if (currentChannel != null && newChannel.Type != ChannelType.Text)
            {
                await ctx.RespondAsync($"{newChannel.Mention} is not a dedicated text channel and cannot be set as the join log channel.");
            }
            else // setting or updating join log
            {
                newChannel ??= ctx.Channel;

                string content = $"Set the join log channel for **{ctx.Guild.Name}** to {newChannel.Mention}?{(currentChannel != null ? $" Current join log channel is {currentChannel.Mention}" : null)}";
                var (userResponse, interaction) = await ctx.WaitForConfirmationInteraction(content);

                if (userResponse is ConfirmationResult.Confirmed)
                {
                    guildEntity.JoinLogChannelId = newChannel.Id;
                    await GuildService.UpdateGuildAsync(guildEntity);

                    content = $":white_check_mark: {newChannel.Mention} is the new joinlog for **{ctx.Guild.Name}**.";
                    await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, content);
                }
                else
                    await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            }
        }
        [Command("purge"), Aliases("delete", "p")]
        [Description("Not implemented.")]
        public async Task PurgeCommand(CommandContext ctx, int count, DiscordMember member = null)
        {
            await ctx.TriggerTypingAsync();
            throw new NotImplementedException();
        }
        [Group("tracking"), Aliases("track", "t")]
        [Description("Commands relating to managing the tracking settings of the guild.")]
        public class TrackingCommands : MutinyBotModule
        {
            [GroupCommand()]
            [Description("Lists the current set tracking settings of the guild. " +
                "**Role tracking** stores the previous roles of a user. " +
                "**Message timestamp tracking** stores *only* the timestamp of a member's last message.")]
            public async Task ListTrackingSettingsCommand(CommandContext ctx)
            {
                throw new NotImplementedException();
            }
            [Command("roles"), Aliases("role", "r")]
            [Description("Enable or disable role tracking for the server. Role tracking stores the previous roles of a user.")]
            public async Task TrackRolesCommand(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                var dbGuild = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);

                string content = $"{(dbGuild.TrackMemberRoles ? "Disable" : "Enable")} role tracking for **{ctx.Guild.Name}**?";
                var (userResponse, interaction) = await ctx.WaitForConfirmationInteraction(content);

                if (userResponse is ConfirmationResult.Confirmed)
                {
                    dbGuild.TrackMemberRoles = !dbGuild.TrackMemberRoles;
                    if (dbGuild.TrackMemberRoles)
                    {
                        var members = await ctx.Guild.GetAllMembersAsync();
                        dbGuild.UpdateGuild(ctx.Guild, members);
                    }
                    await GuildService.UpdateGuildAsync(dbGuild);

                    content = $"{(dbGuild.TrackMemberRoles ? "Enabled" : "Disabled")} role tracking for **{ctx.Guild.Name}**.";
                    await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, content);
                }
                else
                    await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            }
            [Command("timestamps"), Aliases("timestamp", "t")]
            [Description("Enable or disable message timestamp tracking for the server. Timestamp tracking stores *only* the timestamp of a member's last message.")]
            public async Task TrackTimestampCommand(CommandContext ctx)
            {
                throw new NotImplementedException();
            }
        }
    }
}
