using DSharpPlus.CommandsNext;
using MutinyBot.Services;

namespace MutinyBot.Modules
{
    public class MutinyBotModule : BaseCommandModule
    {
        public MutinyBot MutinyBot { protected get; set; }
        public IGuildService GuildService { protected get; set; }
        public IMemberService MemberService { protected get; set; }
    }
}
