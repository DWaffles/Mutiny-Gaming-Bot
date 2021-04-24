using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MutinyBot.Services;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules.Attributes
{
    public class UserNotBannedAttribute : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            IServiceProvider serviceProvider = ctx.CommandsNext.Services;
            IUserService userService = (IUserService)serviceProvider.GetService(typeof(IUserService));
            var userEntity = await userService.GetOrCreateUserAsync(ctx.User.Id);
            return !userEntity.Banned;
        }
    }
}
