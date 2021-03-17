using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Entities;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("joinlog"), Aliases("join", "jl")] // let's mark this class as a command group
    [RequireUserPermissions(DSharpPlus.Permissions.ManageGuild)]
    public class JoinModule : MutinyBotModule
    {
        [Command("set"), Aliases("s")]
        public async Task JoinLogSetChannel(CommandContext ctx, DiscordChannel discordChannel = null)
        {
            if (discordChannel == null)
                discordChannel = ctx.Channel;

            GuildEntity guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);
            guildEntity.JoinLogChannelId = discordChannel.Id;
            await GuildService.UpdateGuildAsync(guildEntity);

            if (guildEntity.JoinLogEnabled)
                await ctx.RespondAsync($"Join notifications will be posted in {discordChannel.Mention}.");
            else
                await ctx.RespondAsync($"When join notifications are enabled they will be posted in {discordChannel.Mention}.");
        }
        [Command("enable"), Aliases("e")]
        public async Task JoinLogEnable(CommandContext ctx)
        {
            GuildEntity guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);
            guildEntity.JoinLogEnabled = true;
            await GuildService.UpdateGuildAsync(guildEntity);

            if (guildEntity.JoinLogChannelId == 0)
                await ctx.RespondAsync("Join notifications have been enabled, please set a channel set to send them in.");
            else
                await ctx.RespondAsync($"Join notiifcations will be posted in <#{guildEntity.JoinLogChannelId}>.");
        }
        [Command("disable"), Aliases("d")]
        public async Task JoinLogDisable(CommandContext ctx)
        {
            GuildEntity guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);
            guildEntity.JoinLogEnabled = false;
            await GuildService.UpdateGuildAsync(guildEntity);

            await ctx.RespondAsync("Join notifications have been disabled.");
        }
    }
}