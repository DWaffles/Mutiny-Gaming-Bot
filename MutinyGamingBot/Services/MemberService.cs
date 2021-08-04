using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Models;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    /// <summary>
    /// The service responsible for interacting with the database in relation to <see cref="MemberModel"/>s.
    /// </summary>
    public class MemberService
    {
        private GuildService GuildService { get; set; }
        public MemberService(GuildService guildService) => GuildService = guildService;
        /// <summary>
        /// Gets or returns a guild members' representation within the database. Returns a full <see cref="MemberModel"/>.
        /// </summary>
        /// <param name="member">The <see cref="DiscordMember"/> object.</param>
        /// <returns>The related <see cref="MemberModel"/>.</returns>
        public async Task<MemberModel> GetOrCreateMemberAsync(DiscordMember member)
        {
            using var context = new MutinyBotDbContext();
            var entity = await context.Members
                .Include(x => x.Guild)
                .SingleOrDefaultAsync(x => x.GuildId == member.Guild.Id && x.MemberId == member.Id);

            if (entity is null)
            {
                var guild = await GuildService.GetOrCreateGuildAsync(member.Guild.Id);
                entity = new MemberModel(member, guild.TrackMemberRoles);
                context.Members.Add(entity);
                await context.SaveChangesAsync();

                entity.Guild = guild;
            }

            return entity;
        }

        /// <summary>
        /// Gets a guild members' representation within the database. Creates and returns a skeleton <see cref="MemberModel"/> if nothing is found.
        /// </summary>
        /// <param name="guildId">The ID of the guild to search within.</param>
        /// <param name="memberId">The ID of the member to search for.</param>
        /// <returns>The related <see cref="MemberModel"/>, without a Guild object.</returns>
        public async Task<MemberModel> GetOrCreateMemberAsync(ulong guildId, ulong memberId)
        {
            using var context = new MutinyBotDbContext();
            var entity = await context.Members
                .Include(x => x.Guild)
                .SingleOrDefaultAsync(x => x.GuildId == guildId && x.MemberId == memberId);

            if (entity is null)
            {
                var guild = await GuildService.GetOrCreateGuildAsync(guildId);
                entity = new MemberModel { GuildId = guildId, MemberId = memberId };
                context.Members.Add(entity);
                await context.SaveChangesAsync();

                entity.Guild = guild;
            }
            return entity;
        }

        /// <summary>
        /// Overwrites the related guild member within the database using the given <see cref="MemberModel"/>.
        /// </summary>
        /// <param name="updatedModel">The <see cref="MemberModel"/> to overwrite with.</param>
        /// <returns></returns>
        public Task UpdateMemberAsync(MemberModel updatedModel)
        {
            using var context = new MutinyBotDbContext();

            var dbMember = context.Members.Find(updatedModel.MemberId, updatedModel.GuildId);
            context.Entry(dbMember).CurrentValues.SetValues(updatedModel);

            var dbGuild = context.Guilds.Find(updatedModel.GuildId);
            context.Entry(dbGuild).CurrentValues.SetValues(updatedModel.Guild);

            return context.SaveChangesAsync();
        }
        public bool RemoveMember(ulong guildId, ulong memberId)
        {
            throw new NotImplementedException();
        }
    }
}
