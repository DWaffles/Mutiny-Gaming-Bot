using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MutinyBot.Modules.Attributes;
using MutinyBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [UserNotBanned]
    public class MutinyBotModule : BaseCommandModule
    {
        public Random Rand { protected get; set; }
        public MutinyBot MutinyBot { protected get; set; }
        public IUserService UserService { protected get; set; }
        public IGuildService GuildService { protected get; set; }
        public IMemberService MemberService { protected get; set; }
        public IPetService PetService { protected get; set; }
        protected static DiscordMember FindMemberByName(IEnumerable<DiscordMember> members, string memberName)
        {
            return members.FirstOrDefault(member => member.DisplayName.Equals(memberName, StringComparison.OrdinalIgnoreCase));
        }
        protected static async Task<DiscordMember> FindMemberByNameAsync(DiscordGuild guild, string memberName)
        {
            var members = await guild.GetAllMembersAsync();
            return members.FirstOrDefault(member => member.DisplayName.Equals(memberName, StringComparison.OrdinalIgnoreCase));
        }
        protected static DiscordEmbed MemberNotFoundEmbed()
        {
            return new DiscordEmbedBuilder
            {
                Title = "User Not Found",
                Description = $"No such user with that nickname or username was found.",
                Color = new DiscordColor(0xFF0000) // red
            };
        }
    }
}