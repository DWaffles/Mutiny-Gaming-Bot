using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Entities;
using System.Threading.Tasks;

namespace MutinyBot.Modules.Moderation
{
    [Group("modlog"), Aliases("moderation", "ml")]
    [RequireUserPermissions(DSharpPlus.Permissions.ManageGuild)]
    public class ModerationModule : MutinyBotModule
    {
        [Command("set"), Aliases("enable", "e", "s")]
        public async Task ModLogSetChannel(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel is null)
                channel = ctx.Channel;

            GuildEntity guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);
            guildEntity.ModerationLogChannelId = channel.Id;
            await GuildService.UpdateGuildAsync(guildEntity);

            await ctx.RespondAsync($"Moderation notifications will be posted in {channel.Mention}.");
        }
        [Command("disable"), Aliases("d")]
        public async Task ModLogDisable(CommandContext ctx)
        {
            GuildEntity guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);
            guildEntity.ModerationLogChannelId = 0;
            await GuildService.UpdateGuildAsync(guildEntity);

            await ctx.RespondAsync("Moderation notifications have been disabled.");
        }
    }
}
