using Microsoft.EntityFrameworkCore;
using MutinyBot.Database;
using MutinyBot.Models;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    /// <summary>
    /// The service responsible for interacting with the database in relation to <see cref="UserModel"/>s.
    /// </summary>
    public class UserService
    {
        public UserService() { }

        /// <summary>
        /// Gets a users' representation within the database. Creates and returns a <see cref="UserModel"/> if nothing is found.
        /// </summary>
        /// <param name="userId">The ID of the user to search for.</param>
        /// <returns>The related <see cref="UserModel"/>.</returns>
        public async Task<UserModel> GetOrCreateUserAsync(ulong userId)
        {
            using var context = new MutinyBotDbContext();
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserId == userId);

            if (user is null)
            {
                user = new UserModel { UserId = userId };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            return user;
        }

        /// <summary>
        /// Overwrites the related user within the database using the given <see cref="UserModel"/>.
        /// </summary>
        /// <param name="model">The <see cref="UserModel"/> to overwrite with.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public Task UpdateUserAsync(UserModel updatedModel)
        {
            using var context = new MutinyBotDbContext();

            var dbMember = context.Users.Find(updatedModel.UserId);
            context.Entry(dbMember).CurrentValues.SetValues(updatedModel);

            return context.SaveChangesAsync();
        }
        public bool RemoveUser(ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
