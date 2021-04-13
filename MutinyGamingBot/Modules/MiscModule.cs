using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class MiscModule : MutinyBotModule
    {
        [Command("ping"), Aliases("p")]
        [Description("Returns the bot's ping to Discord.")] // displayed when help is invoked
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"Pong: {ctx.Client.Ping}ms");
        }
        [Command("coin"), Aliases("coinflip")]
        [Description("Flips a digital coin.")]
        public async Task CoinFlip(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            int result = Rng.Next(2);
            if (result == 0) //heads
                await ctx.RespondAsync($"Heads!");
            else //tails
                await ctx.RespondAsync($"Tails, or something.");
        }
        //disclaimer
    }
}
