using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Entities;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    public interface IMemberService
    {
        Task<MemberEntity> GetOrCreateMemberAsync(DiscordMember discordMember);
        Task<MemberEntity> GetNewMemberJoinedAsync(DiscordMember discordMember);
        Task UpdateMemberAsync(MemberEntity member);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="discordMember">The DiscordMember object of the member who has joined.</param>
        /// <returns></returns>
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
        public Task UpdateMemberAsync(MemberEntity memberEntity)
        {
            _ = dbContext.Members.Update(memberEntity);
            return dbContext.SaveChangesAsync();
        }
        public async Task RemoveMemberAsync(ulong guildId, ulong memberId)
        {
            throw new NotImplementedException();
        }
    }
}
