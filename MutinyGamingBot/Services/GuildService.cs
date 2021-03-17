using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Entities;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    public interface IGuildService
    {
        public Task<GuildEntity> GetOrCreateGuildAsync(ulong guildId);
        public Task UpdateGuildAsync(GuildEntity guildModel);
        public Task RemoveGuildAsync(ulong guildId);
    }
    public class GuildService : IGuildService
    {
        private readonly MutinyBotDbContext dbContext;
        public GuildService(MutinyBotDbContext dbContext)
        {
            this.dbContext = dbContext;
            Console.WriteLine("GUILDSERVICE");
        }
        public async Task<GuildEntity> GetOrCreateGuildAsync(ulong guildId)
        {
            GuildEntity guildEntity = await dbContext.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (guildEntity == null)
            {
                guildEntity = new GuildEntity { GuildId = guildId };
                dbContext.Guilds.Add(guildEntity);
                await dbContext.SaveChangesAsync();
            }
            return guildEntity;
        }
        public Task UpdateGuildAsync(GuildEntity guildEntity)
        {
            _ = dbContext.Guilds.Update(guildEntity);
            return dbContext.SaveChangesAsync();
        }
        public Task RemoveGuildAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
