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
    [RequireOwner, Hidden]
    public class CreatorModule : MutinyBotModule
    {
        [Command("quit"), Aliases("shutdown", "q")]
        [Description("Command will shut down the bot.")]
        public async Task QuitCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("Quitting application.");
            await ctx.Client.DisconnectAsync();
            Log.CloseAndFlush();
            Environment.Exit(0);
        }
        [Command("echo"), Aliases("repeat", "e")]
        [Description("Makes the bot echo a message. Deletes command message.")]
        public async Task EchoCommand(CommandContext ctx, [RemainingText, Description("Message to echo.")] string message)
        {
            if (message.Length == 0)
                return;

            await ctx.TriggerTypingAsync();
            if (!ctx.Channel.IsPrivate)
            {
                if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasFlag(Permissions.ManageMessages))
                    await ctx.Message.DeleteAsync();
            }
            await ctx.RespondAsync(message);
        }
        [Command("botban")]
        public async Task BotBanHandlerCommand(CommandContext ctx, DiscordUser user)
        {
            DiscordEmbed embed;
            var userEntity = await UserService.GetOrCreateUserAsync(user.Id);

            if (!userEntity.IsBanned)
                embed = await GetBanConfirmEmbed(ctx, user);
            else
                embed = await GetRevokeBanConfirmEmbed(ctx, user);

            var (response, buttonPress) = await ctx.WaitForConfirmationInteraction(embed);

            if (response is ConfirmationResult.Confirmed)
            {
                await buttonPress.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                userEntity.IsBanned = !userEntity.IsBanned;
                await UserService.UpdateUserAsync(userEntity);

                string content = userEntity.IsBanned ? ":white_check_mark: Banned" : ":white_check_mark: Ban Revoked";
                await buttonPress.Interaction.EditOriginalResponseAsync(content);
            }
        }
        //EXPORT
        //PETOWNERS

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
    [Group("debug"), Aliases("d")]
    [Description("Debug commands."), Hidden, RequireOwner]
    public class DebugModule : MutinyBotModule
    {
        [Command("embed")]
        public async Task EmbedDebugCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(name: "Author Field *[Can Be Linked]*", iconUrl: ctx.Member.AvatarUrl, url: ctx.Member.AvatarUrl)
                .WithTitle("Title Field *[No Markdown]*")
                .WithDescription("Description")
                .WithColor(GetBotColor());

            await ctx.RespondAsync(embed: embed);
        }
        [Command("timestamp")]
        public async Task TestTimestampCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var entity = await MemberService.GetOrCreateMemberAsync(ctx.Guild.Id, ctx.Member.Id);
            await ctx.RespondAsync($"<t:{entity.LastMessageTimestampRaw}:R>");
        }
    }
}
