using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireOwner]
    [Description("Creator only commands.")]
    [Hidden]
    public class CreatorModule : MutinyBotModule
    {
        [Command("quit"), Aliases("q", "shutdown"), Description("Bot will shut down.")]
        public async Task Quit(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("Quitting application.");
            Environment.Exit(0);
        }
        [Command("echo"), Aliases("e", "repeat")]
        [Description("Makes the bot echo a message. Deletes command message.")]
        public async Task Echo(CommandContext ctx, [RemainingText, Description("Message to echo.")] string message)
        {
            if (message.Length == 0)
                return;

            await ctx.TriggerTypingAsync();
            if (!ctx.Channel.IsPrivate)
            {
                if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasFlag(DSharpPlus.Permissions.ManageMessages)) //alt. ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasFlag(DSharpPlus.Permissions.ManageMessages
                    await ctx.Message.DeleteAsync();
            }
            await ctx.RespondAsync(message);
        }
        // Change to allow choosing of activity type
        [Command("status"), Aliases("s"), Description("Sets the bot status.")]
        public async Task Status(CommandContext ctx, [RemainingText, Description("Status to set.")] string status)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(status));
            await ctx.RespondAsync($"Status set to {status}");
        }
        //set activity
    }
    [Group("debug"), Aliases("d")]
    [Description("Debug commands."), Hidden, RequireOwner]
    public class DebugModule : MutinyBotModule
    {
        [Command("membercache"), Aliases("mc")]
        public async Task Debug(CommandContext ctx)
        {
            string test = $"Members Count: {ctx.Guild.Members.Count}";
            foreach (var member in ctx.Guild.Members.Values)
            {
                test += $"\n{member.DisplayName}\t{member.Username}";
            }
            await ctx.RespondAsync(test);

            var list = await ctx.Guild.GetAllMembersAsync();
            test = $"Members Count: {list.Count}";
            foreach (var member in list)
            {
                test += $"\n{member.DisplayName}\t{member.Username}\t{member.Roles.Count()}";
            }
            await ctx.RespondAsync(test);
        }
        [Command("rtext"), Aliases("rt")]
        public async Task RemainingText(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.RespondAsync(text);
        }
        [Command("add")]
        public async Task AddNumber(CommandContext ctx, int num1, int num2)
        {
            await ctx.RespondAsync((num1 + num2).ToString());
        }
    }
}
