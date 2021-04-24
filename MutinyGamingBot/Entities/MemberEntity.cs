using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MutinyBot.Entities
{
    public class MemberEntity : DbEntity
    {
        public ulong MemberId { get; set; }
        public ulong GuildId { get; set; }
        public int TimesJoined { get; set; } = 1;
        public int TimesMuted { get; set; }
        public bool CurrentMember { get; set; } = true;
        public Dictionary<ulong, bool> RoleDictionary { get; set; } = new Dictionary<ulong, bool>();
        public MemberEntity() { }
        public MemberEntity(DiscordMember member)
        {
            MemberId = member.Id;
            GuildId = member.Guild.Id;
            TimesJoined = 1;
            foreach (DiscordRole role in member.Roles)
            {
                RoleDictionary.Add(role.Id, true);
            }
        }
        public void UpdateEntity(DiscordMember member)
        {
            foreach (var key in RoleDictionary.Keys.ToList())
            {
                RoleDictionary[key] = false;
            }
            foreach (DiscordRole role in member.Roles)
            {
                if (RoleDictionary.ContainsKey(role.Id))
                {
                    RoleDictionary[role.Id] = true;
                }
                else
                {
                    RoleDictionary.Add(role.Id, true);
                }
            }
        }
    }
}