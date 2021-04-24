using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using MutinyBot.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        [Command("botban"), Description("Bans the user from interacting with the bot.")]
        public async Task BotBanUser(CommandContext ctx, DiscordUser user)
        {
            await ctx.TriggerTypingAsync();

            var userEntity = await UserService.GetOrCreateUserAsync(user.Id);
            userEntity.Banned = true;
            await UserService.UpdateUserAsync(userEntity);

            await ctx.RespondAsync($"User has been added to the ban list.");
        }
        [Command("unbotban"), Description("Un-Bans the user from interacting with the bot.")]
        public async Task UnBotBanUser(CommandContext ctx, DiscordUser user)
        {
            await ctx.TriggerTypingAsync();

            var userEntity = await UserService.GetOrCreateUserAsync(user.Id);
            userEntity.Banned = false;
            await UserService.UpdateUserAsync(userEntity);

            await ctx.RespondAsync($"User has been removed from the ban list.");
        }
        // Change to allow choosing of activity type
        [Command("status"), Aliases("s"), Description("Sets the bot status.")]
        public async Task Status(CommandContext ctx, [RemainingText, Description("Status to set.")] string status)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(status));
            await ctx.RespondAsync($"Status set to {status}");
        }
        [Command("export")]
        public async Task ExportMembers(CommandContext ctx, string fileName = "output.csv")
        {
            await ctx.TriggerTypingAsync();

            if (!fileName.EndsWith(".csv"))
                fileName += ".csv";

            string output = "Discord Username, Guild Nickname, User Id, Joined, Num Roles, Current Roles";

            var members = await ctx.Guild.GetAllMembersAsync();
            List<string> userStrings = new List<string>(); //ConcurrentBag?
            foreach (var member in members) //Parallel.ForEach?
            {
                string userString = "";
                //Username
                userString = UtilityHelpers.SanitizeString(member.Username) + ",";
                //Nickname
                userString = UtilityHelpers.SanitizeString(member.Nickname ?? member.Username) + ",";
                //UserId
                userString += member.Id + ",";
                //Joined
                userString += member.JoinedAt.UtcDateTime.ToString("g") + ",";
                //Num Roles
                userString += member.Roles.Count() + ",";
                //Current Roles
                var orderedRoles = member.Roles.ToList().OrderByDescending(x => x.Position);
                userString += String.Join("|", orderedRoles.Select(x => UtilityHelpers.SanitizeString(x.Name)));
                //Previous Roles?

                userStrings.Add(userString);
            }
            output += "\n" + String.Join("\n", userStrings);

            Console.WriteLine($"Writing to {fileName}.");
            File.WriteAllText(fileName, output, new UTF8Encoding(false));
            Console.WriteLine($"Outputted matched mods to {fileName}.");
        }
    }
    [Group("debug"), Aliases("d")]
    [Description("Debug commands."), Hidden, RequireOwner]
    public class DebugModule : MutinyBotModule
    {
        [Command("mute")]
        public async Task MuteDebug(CommandContext ctx, DiscordMember member, TimeSpan muteTime, string reason = null)
        {
            await ctx.TriggerTypingAsync();

            string response = String.Empty;

            response = String.Join("\n", response, $"Member: {member.Mention}");

            if (String.IsNullOrEmpty(reason))
                response = String.Join("\n", response, $"Reason: None given");
            else
                response = String.Join("\n", response, $"Reason: {reason}");

            var guildEntity = await GuildService.GetOrCreateGuildAsync(ctx.Guild.Id);

            DiscordRole muteRole = ctx.Guild.GetRole(guildEntity.MuteRoleId);
            if (muteRole is null)
                response = String.Join("\n", response, $"Mute Role: Not Set");
            else
            {
                response = String.Join("\n", response, $"Mute Role: {muteRole.Mention}");
                response = String.Join("\n", response, $"Mute Role Position: {muteRole.Position}");
            }

            DiscordChannel modChannel = ctx.Guild.GetChannel(guildEntity.ModerationLogChannelId);
            if (modChannel is null)
                response = String.Join("\n", response, $"Moderation Channel: Not Set");
            else
                response = String.Join("\n", response, $"Moderation Channel: {modChannel.Mention}");

            response = String.Join("\n", response, $"Time Period: {muteTime.Humanize()}");

            response = String.Join("\n", response, $"Bot Highest Role Position: {ctx.Guild.CurrentMember.Roles.Max(role => role.Position)}");

            await ctx.RespondAsync(response);
        }
    }
}
