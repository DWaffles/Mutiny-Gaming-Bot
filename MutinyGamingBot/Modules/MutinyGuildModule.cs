using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Modules.Attributes;
using MutinyBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class MutinyGuildModule : MutinyBotModule
    {
        public MutinyBot _mutinyBot { private get; set; }
        public IGuildService _guildService { private get; set; }
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