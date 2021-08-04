using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class MiscModule : MutinyBotModule
    {
        [Command("ping"), Aliases("p")]
        [Description("Returns the bot's ping to Discord.")]
        public async Task PingCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"Pong: {ctx.Client.Ping}ms");
        }
        [Command("coin"), Aliases("coinflip")]
        [Description("Flips a digital coin.")]
        public async Task CoinFlipCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            int result = Rand.Next(2);
            if (result == 0) //heads
                await ctx.RespondAsync($"Heads!");
            else //tails
                await ctx.RespondAsync($"Tails, or something.");
        }
        [Command("profile"), Aliases("picture", "pfp")]
        [Description("Gets a large version of someone's profile picture.")]
        public async Task ProfileImageCommand(CommandContext ctx, DiscordUser user = null)
        {
            user ??= ctx.User;

            await ctx.TriggerTypingAsync();
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{user.Username}#{user.Discriminator}")
                .WithImageUrl(user.AvatarUrl)
                .WithFooter($"User ID: {user.Id}")
                .WithColor(GetBotColor());

            await ctx.RespondAsync(embed: embed);
        }
    }
}
