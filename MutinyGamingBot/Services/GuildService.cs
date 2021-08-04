using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Models;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    /// <summary>
    /// The service responsible for interacting with the database in relation to <see cref="GuildModel"/>s.
    /// </summary>
    public class GuildService
    {
        public GuildService() { }
        /// <summary>
        /// Gets a guilds' representation within the database. Creates and returns a filled <see cref="GuildModel"/> if nothing is found.
        /// </summary>
        /// <param name="guild">The DiscordGuild object.</param>
        /// <returns>The related <see cref="GuildModel"/>, includes <see cref="MemberModel">MemberModels</see>.</returns>
        public async Task<GuildModel> GetOrCreateGuildAsync(DiscordGuild guild)
        {
            using var context = new MutinyBotDbContext();
            GuildModel entity = await context.Guilds
                .Include(g => g.Members)
                .SingleOrDefaultAsync(x => x.GuildId == guild.Id);

            if (entity is null)
            {
                entity = new GuildModel(guild);
                context.Guilds.Add(entity);
                await context.SaveChangesAsync();
            }
            return entity;
        }

        /// <summary>
        /// Gets a guilds' representation within the database. Creates and returns a skeleton <see cref="GuildModel"/> if nothing is found.
        /// </summary>
        /// <param name="guildId">The ID of the guild to search for.</param>
        /// <param name="includeMembers">Whether to include <see cref="MemberModel">MemberModels</see> in the returned <see cref="GuildModel"/>.</param>
        /// <returns>The related <see cref="GuildModel"/>.</returns>
        public async Task<GuildModel> GetOrCreateGuildAsync(ulong guildId, bool includeMembers = false)
        {
            using var context = new MutinyBotDbContext();
            GuildModel entity;
            if (includeMembers)
            {
                entity = await context.Guilds
                    .Include(g => g.Members)
                    .SingleOrDefaultAsync(x => x.GuildId == guildId);
            }
            else
            {
                entity = await context.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            }

            if (entity is null)
            {
                entity = new GuildModel { GuildId = guildId };
                context.Guilds.Add(entity);
                await context.SaveChangesAsync();
            }
            return entity;
        }

        /// <summary>
        /// Overwrites the related guild within the database using the given <see cref="GuildModel"/>.
        /// </summary>
        /// <param name="updatedModel">The <see cref="GuildModel"/> to overwrite with.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public async Task UpdateGuildAsync(GuildModel updatedModel)
        {
            using var context = new MutinyBotDbContext();

            var dbGuild = context.Guilds.Find(updatedModel.GuildId);
            foreach (var updatedMember in updatedModel.Members)
            {
                var dbMember = context.Members.Find(updatedMember.MemberId, updatedMember.GuildId);
                if (dbMember != null)
                {
                    context.Entry(dbMember).CurrentValues.SetValues(updatedMember);
                }
                else
                {
                    context.Members.Add(updatedMember);
                }
            }
            context.Entry(dbGuild).CurrentValues.SetValues(updatedModel);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public bool RemoveGuild(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
