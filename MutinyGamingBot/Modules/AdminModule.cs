using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("admin"), Aliases("a")] // let's mark this class as a command group
    [Description("Administrative commands.")] // give it a description for help purposes
    public class AdminModule : MutinyBotModule
    {
        //[Command("Mute"), Aliases(), Description()]
        public async Task MuteCommand(CommandContext ctx, DiscordMember discordMember)
        {
            await ctx.TriggerTypingAsync();

            //[prefix]mute @Member 5m
        }
    }
}
