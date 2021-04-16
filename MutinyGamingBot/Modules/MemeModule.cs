using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Entities;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("meme"), Aliases("m")] // let's mark this class as a command group
    public class MemeModule : MutinyBotModule
    {
        [Command("angloteachart"), Aliases("tea", "teachart")]
        [Description("Returns the bot's ping to Discord.")] // displayed when help is invoked
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"https://i.imgur.com/6v0VFI4.png");
        }
    }
}
