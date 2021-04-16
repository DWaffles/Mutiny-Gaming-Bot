using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    public interface IMemberService
    {
        Task<MemberEntity> GetOrCreateMemberAsync(DiscordMember discordMember);
        List<MemberEntity> GetAllMembers(ulong guildId);
        Task<MemberEntity> GetNewMemberJoinedAsync(DiscordMember discordMember);
        Task UpdateMemberAsync(MemberEntity member);
        Task UpdateMembersAsync(List<MemberEntity> memberList);
        Task RemoveMemberAsync(ulong guildId, ulong memberId);
    }
    public class MemberService : IMemberService
    {
        private readonly MutinyBotDbContext dbContext;
        public MemberService(MutinyBotDbContext dbContext)
        {
            this.dbContext = dbContext;
            Console.WriteLine("MEMBERSERVICE");
        }
        public async Task<MemberEntity> GetOrCreateMemberAsync(DiscordMember discordMember)
        {
            MemberEntity member = await dbContext.Members.SingleOrDefaultAsync(x => x.MemberId == discordMember.Id && x.GuildId == discordMember.Guild.Id);
            if (member == null)
            {
                member = new MemberEntity(discordMember);
                _ = dbContext.Members.Add(member);
                _ = await dbContext.SaveChangesAsync();
            }
            return member;
        }
        public async Task<MemberEntity> GetNewMemberJoinedAsync(DiscordMember discordMember)
        {
            MemberEntity member = await dbContext.Members.SingleOrDefaultAsync(x => x.MemberId == discordMember.Id && x.GuildId == discordMember.Guild.Id);
            if (member == null)
            {
                member = new MemberEntity(discordMember);
                _ = dbContext.Members.Add(member);
                _ = await dbContext.SaveChangesAsync();
            }
            else
            {
                member.TimesJoined++;
                _ = await dbContext.SaveChangesAsync();
            }
            return member;
        }
        public List<MemberEntity> GetAllMembers(ulong guildId)
        {
            return dbContext.Members.Where(member => member.GuildId == guildId).ToList();
        }
        public Task UpdateMemberAsync(MemberEntity memberEntity)
        {
            _ = dbContext.Members.Update(memberEntity);
            return dbContext.SaveChangesAsync();
        }
        public Task UpdateMembersAsync(List<MemberEntity> memberList)
        {
            dbContext.Members.UpdateRange(memberList);
            return dbContext.SaveChangesAsync();
        }
        public async Task RemoveMemberAsync(ulong guildId, ulong memberId)
        {
            throw new NotImplementedException();
        }
    }
}
