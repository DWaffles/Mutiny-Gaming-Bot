using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules.Attributes
{
    public class RequireMutinyGuildAttribute : CheckBaseAttribute
    {
        public const ulong MutinyGuildId = 521821990403702785;
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
            => Task.FromResult(ctx.Guild.Id == MutinyGuildId);
    }
}
