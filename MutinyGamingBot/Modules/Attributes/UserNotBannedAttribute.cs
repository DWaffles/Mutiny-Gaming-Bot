using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MutinyBot.Services;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    /// <summary>
    /// Defines that users bot banned cannot trigger this command.
    /// </summary>
    public class UserNotBannedAttribute : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var userService = (UserService)ctx.Services.GetRequiredService(typeof(UserService));
            var user = await userService.GetOrCreateUserAsync(ctx.User.Id);
            return !user.IsBanned;
        }
    }
}