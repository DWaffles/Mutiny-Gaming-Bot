using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Group("tag"), Aliases("tags")]
    [Description("Tags")]
    public class TagModule : MutinyBotModule
    {
        [GroupCommand()]
        public async Task TagCommand(CommandContext ctx) 
        {
            await ctx.TriggerTypingAsync();
            throw new NotImplementedException();
        }
        //Don't mention people or @everyone/@here
        //raw
        //create
        //edit/update
        //list
    }
}
