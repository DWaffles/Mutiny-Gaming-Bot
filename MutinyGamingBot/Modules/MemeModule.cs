using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("meme"), Aliases("m")] // mute has
    public class MemeModule : MutinyBotModule
    {
        [Command("teachart"), Aliases("tea")]
        [Description("Returns the bot's ping to Discord.")] // displayed when help is invoked
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"https://i.imgur.com/6v0VFI4.png");
        }
    }
}