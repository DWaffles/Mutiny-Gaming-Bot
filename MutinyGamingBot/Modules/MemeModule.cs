using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("meme"), Hidden]
    public class MemeModule : MutinyBotModule
    {
        [Command("tea"), Aliases("chart")]
        [Description("Returns the Anglo's favorite chart.")]
        public async Task TeaChartCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"https://i.imgur.com/6v0VFI4.png");
        }
        // Its been 0 days since command channel
        // bowling ball
    }
}
