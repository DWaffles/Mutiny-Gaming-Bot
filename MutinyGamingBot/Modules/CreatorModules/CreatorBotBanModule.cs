using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Enums;
using MutinyBot.Extensions;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class CreatorBotBanModule : CreatorModule
    {
        [Command("botban")]
        public async Task BotBanHandlerCommand(CommandContext ctx, DiscordMember member)
        {
            await BotBanHandlerCommand(ctx, member as DiscordUser);
        }
        [Command("botban")]
        public async Task BotBanHandlerCommand(CommandContext ctx, [RemainingText] DiscordUser user)
        {
            DiscordEmbed embed;
            var userEntity = await UserService.GetOrCreateUserAsync(user.Id);

            if (!userEntity.IsBanned)
                embed = await GetBanConfirmEmbed(ctx, user);
            else
                embed = await GetRevokeBanConfirmEmbed(ctx, user);

            var (userResponse, interaction) = await ctx.WaitForConfirmationInteraction(embed);

            if (userResponse is ConfirmationResult.Confirmed)
            {
                await interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                userEntity.IsBanned = !userEntity.IsBanned;
                await UserService.UpdateUserAsync(userEntity);

                string content = userEntity.IsBanned ? ":white_check_mark: Banned" : ":white_check_mark: Ban Revoked";
                await interaction.EditOriginalResponseAsync(content);
            }
            else
                await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        }
        
        #region EmbedFunctions
        private async Task<DiscordEmbed> GetBanConfirmEmbed(CommandContext ctx, DiscordUser user)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Bot Ban {user.Username}#{user.Discriminator}?")
                .WithDescription($"User to Ban: {user.Mention}")
                .WithThumbnail(user.AvatarUrl)
                .WithTimestamp(DateTime.Now)
                .AddField("Is Bot?", $"{user.IsBot}", true)
                .WithColor(GetBotColor());

            if (ctx.Guild != null)
            {
                try
                {
                    _ = await ctx.Guild.GetMemberAsync(user.Id);
                    embed.AddField($"Member of {ctx.Guild.Name}?", "True", true);
                }
                catch
                {
                    embed.AddField($"Member of {ctx.Guild.Name}?", "False", true);
                }
            }

            return embed;
        }
        private async Task<DiscordEmbed> GetRevokeBanConfirmEmbed(CommandContext ctx, DiscordUser user)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Revoke Bot Ban of {user.Username}#{user.Discriminator}?")
                .WithDescription($"User to Unban: {user.Mention}")
                .WithThumbnail(user.AvatarUrl)
                .WithTimestamp(DateTime.Now)
                .AddField("Is Bot?", $"{user.IsBot}", true)
                .WithColor(GetBotColor());

            if (ctx.Guild != null)
            {
                var userIsMember = await ctx.Guild.GetMemberAsync(user.Id) is null ? "False" : "True";
                embed.AddField($"Member of {ctx.Guild.Name}?", userIsMember, true);
            }

            return embed;
        }
        #endregion
    }
}
