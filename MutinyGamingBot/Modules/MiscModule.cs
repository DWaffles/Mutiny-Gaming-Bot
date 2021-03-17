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
        //disclaimer
    }
}
