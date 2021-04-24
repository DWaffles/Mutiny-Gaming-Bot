using DSharpPlus.CommandsNext;
using MutinyBot.Modules.Attributes;
using MutinyBot.Services;
using System;

namespace MutinyBot.Modules
{
    [UserNotBanned]
    public class MutinyBotModule : BaseCommandModule
    {
        public Random Rng { protected get; set; }
        public MutinyBot MutinyBot { protected get; set; }
        public IUserService UserService { protected get; set; }
        public IGuildService GuildService { protected get; set; }
        public IMemberService MemberService { protected get; set; }
    }
}