using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Enums;
using MutinyBot.Extensions;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireOwner, Hidden]
    public class CreatorModule : MutinyBotModule
    {
        [Command("quit"), Aliases("shutdown", "q")]
        [Description("Command will shut down the bot.")]
        public async Task QuitCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("Quitting application.");
            await ctx.Client.DisconnectAsync();
            Log.CloseAndFlush();
            Environment.Exit(0);
        }
        [Command("echo"), Aliases("repeat", "e")]
        [Description("Makes the bot echo a message. Deletes command message.")]
        public async Task EchoCommand(CommandContext ctx, [RemainingText, Description("Message to echo.")] string message)
        {
            if (message.Length == 0)
                return;

            await ctx.TriggerTypingAsync();
            if (!ctx.Channel.IsPrivate)
            {
                if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasFlag(Permissions.ManageMessages))
                    await ctx.Message.DeleteAsync();
            }
            await ctx.RespondAsync(message);
        }
    }
    [Group("debug"), Aliases("d")]
    [Description("Debug commands."), Hidden, RequireOwner]
    public class DebugModule : MutinyBotModule
    {
        [Command("embed")]
        public async Task EmbedDebugCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(name: "Author Field *[Can Be Linked]*", iconUrl: ctx.Member.AvatarUrl, url: ctx.Member.AvatarUrl)
                .WithTitle("Title Field *[No Markdown]*")
                .WithDescription("Description")
                .WithColor(GetBotColor());

            await ctx.RespondAsync(embed: embed);
        }
        [Command("timestamp")]
        public async Task TestTimestampCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var entity = await MemberService.GetOrCreateMemberAsync(ctx.Guild.Id, ctx.Member.Id);
            await ctx.RespondAsync($"<t:{entity.LastMessageTimestampRaw}:R>");
        }
        [Command("exportrole"), Aliases("export", "e")]
        public async Task ExportCommand(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            var members = (await ctx.Guild.GetAllMembersAsync()).Where(x => x.Roles.Contains(role)).OrderBy(m => m.DisplayName);

            StringBuilder stringBuilder = new();
            foreach (var member in members)
            {
                    stringBuilder.AppendLine($"{member.DisplayName}");
            }
            File.WriteAllText($"data{Path.DirectorySeparatorChar}output-{role.Name}.txt", stringBuilder.ToString());
        }
    }
}