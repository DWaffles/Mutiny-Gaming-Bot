using MutinyBot.Entities;
using System.Collections.Generic;

namespace MutinyBot.Models
{
    public class GuildModel
    {
        public Dictionary<ulong, MemberEntity> MemberDictionary { get; private set; } = new Dictionary<ulong, MemberEntity>(); //Use MemberModel
        public ulong GuildId { get; private set; }
        public string GuildName { get; private set; }
    }
}