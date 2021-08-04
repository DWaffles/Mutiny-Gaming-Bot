using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
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
        public UserService UserService { protected get; set; }
        public GuildService GuildService { protected get; set; }
        public MemberService MemberService { protected get; set; }
        public PetService PetService { protected get; set; }
        public MutinyBot MutinyBot { protected get; set; }
        protected DiscordColor GetBotColor()
        {
            return MutinyBot.GetBotColor();
        }
        protected static DiscordMember GetMemberByName(IEnumerable<DiscordMember> members, string memberName)
        {
            return members.FirstOrDefault(member => member.Nickname.Equals(memberName, StringComparison.OrdinalIgnoreCase)
            || member.Username.Equals(memberName, StringComparison.OrdinalIgnoreCase));
        }
        protected static async Task<DiscordMember> GetMemberByNameAsync(DiscordGuild guild, string memberName)
        {
            var members = await guild.GetAllMembersAsync();
            return members.FirstOrDefault(member => member.Nickname.Equals(memberName, StringComparison.OrdinalIgnoreCase)
            || member.Username.Equals(memberName, StringComparison.OrdinalIgnoreCase));
        }
        protected static DiscordEmbed GetMemberNotFoundEmbed()
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
