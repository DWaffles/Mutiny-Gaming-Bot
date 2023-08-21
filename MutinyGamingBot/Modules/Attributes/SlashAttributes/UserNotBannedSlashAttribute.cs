using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using MutinyBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    /// <summary>
    /// Defines that users bot banned cannot trigger this command.
    /// </summary>
    public class UserNotBannedSlashAttribute : SlashCheckBaseAttribute
    {
        // InteractionContext instead of BaseContext for official D#+ slash command package.
        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            var userService = (UserService)ctx.Services.GetRequiredService(typeof(UserService));
            var user = await userService.GetOrCreateUserAsync(ctx.User.Id);
            return !user.IsBanned;
        }
    }
}
