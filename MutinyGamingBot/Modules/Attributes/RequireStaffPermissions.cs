using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules.Attributes
{
    public class RequireStaffPermissions : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            throw new NotImplementedException();
        }
    }
}
