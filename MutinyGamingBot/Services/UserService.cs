using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Entities;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    public interface IUserService
    {
        public Task<UserEntity> GetOrCreateUserAsync(ulong userId);
        public Task UpdateUserAsync(UserEntity userEntity);
        public Task RemoveUserAsync(ulong userId);
    }
    public class UserService : IUserService
    {
        private readonly MutinyBotDbContext dbContext;
        public UserService(MutinyBotDbContext dbContext)
        {
            this.dbContext = dbContext;
            Console.WriteLine("USERSERVICE");
        }
        public async Task<UserEntity> GetOrCreateUserAsync(ulong userId)
        {
            UserEntity user = await dbContext.Users.SingleOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                user = new UserEntity { UserId = userId };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
            return user;
        }
        public Task UpdateUserAsync(UserEntity userEntity)
        {
            _ = dbContext.Users.Update(userEntity);
            return dbContext.SaveChangesAsync();
        }
        public Task RemoveUserAsync(ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
