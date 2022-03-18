using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using MutinyBot.Services;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    /// <summary>
    /// Defines that only users who are authorized to add pet images can use this command.
    /// </summary>
    public class IsPetOwnerSlashAttribute : SlashCheckBaseAttribute
    {
        public override async Task<bool> ExecuteChecksAsync(BaseContext ctx)
        {
            var userService = (UserService)ctx.Services.GetRequiredService(typeof(UserService));
            var user = await userService.GetOrCreateUserAsync(ctx.User.Id);
            return user.IsPetOwner;
        }
    }
}
