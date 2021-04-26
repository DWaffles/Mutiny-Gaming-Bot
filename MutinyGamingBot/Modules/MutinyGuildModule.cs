using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class MutinyGuildModule : MutinyBotModule
    {
        [Command("invite"), Aliases("discordInvite")]
        [Description("Get's the server Discord invite."), RequireGuild()]
        public async Task GetInvite(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"https://discord.gg/G4kH3sT");
        }
        //trainings
    }
}