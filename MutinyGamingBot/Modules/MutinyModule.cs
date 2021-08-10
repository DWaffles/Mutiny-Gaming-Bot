using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    class MutinyModule
    {
        [Command("invite"), Aliases("discord")]
        [Description("Gets' Mutiny Gamings' Discord invite."), RequireGuild()]
        public async Task GetMutinyInviteCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"https://discord.gg/G4kH3sT");
        }
        //trainings
        //sign up
    }
}
