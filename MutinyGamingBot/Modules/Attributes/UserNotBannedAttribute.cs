using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MutinyBot.Services;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules.Attributes
{
    public class UserNotBannedAttribute : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            IUserService userService = (IUserService)ctx.Services.GetRequiredService(typeof(IUserService));
            var userEntity = await userService.GetOrCreateUserAsync(ctx.User.Id);
            return !userEntity.Banned;
        }
    }
}
