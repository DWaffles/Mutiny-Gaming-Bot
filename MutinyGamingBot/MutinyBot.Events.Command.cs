using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Humanizer;
using MutinyBot.Extensions;
using MutinyBot.Modules;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands.Attributes;


namespace MutinyBot
{
    public partial class MutinyBot
    {
        private void RegisterCommandEvents()
        {
            Commands.CommandExecuted += CommandExecuted;
            Commands.CommandErrored += CommandErrored;

            SlashCommands.SlashCommandInvoked += SlashCommandInvoked;
            SlashCommands.SlashCommandExecuted += SlashCommandExecuted;
            SlashCommands.SlashCommandErrored += SlashCommandErrored;
        }

        #region EventHandlers
        #region CommandHandlers
        private Task CommandExecuted(CommandsNextExtension _, CommandExecutionEventArgs e)
        {
            Log.Logger.Information($"[COMMAND] {e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");
            return Task.CompletedTask;
        }
        private async Task CommandErrored(CommandsNextExtension commands, CommandErrorEventArgs e)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(0xFF0000));

            switch (e.Exception)
            {
                case ArgumentException _:
                    if (!e.Exception.Message.Equals("Could not find a suitable overload for the command."))
                        goto default;

                    embed.Title = "Invalid Arguments Given";
                    embed.Description = $"The arguments given for the `{e.Command.QualifiedName}` command is not valid." +
                        $" {(e.Command.Aliases.Any() ? $"The aliases for this command are: {String.Join(", ", e.Command.Aliases.Select(x => $"`{x}`"))}." : null)}" +
                        $"\n\n" +
                        $"An argument wrapped by `<>` denotes a mandatory argument, while those in `[]` is optional. As a reminder, my prefixes are any of the following: {String.Join(", ", Config.Discord.CommandPrefixes.Select(x => $"`{x}`"))}.";

                    var sBuilder = new StringBuilder();
                    var commandOverloads = e.Command.Overloads.OrderByDescending(x => x.Priority);
                    CommandOverload overload;

                    for (int i = 1; i <= commandOverloads.Count(); i++)
                    {
                        overload = commandOverloads.ElementAt(i - 1);

                        sBuilder.Append($"`{e.Command.QualifiedName}");

                        foreach (var arg in overload.Arguments)
                            sBuilder.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                        sBuilder.Append("`\n");

                        foreach (var arg in overload.Arguments)
                            sBuilder.Append('`').Append(arg.Name).Append(" (").Append(Commands.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');

                        embed.AddField($"{i.ToOrdinalWords().CapitalizeFirst()} Overload", sBuilder.ToString().Trim(), false);
                        sBuilder.Clear();
                    }
                    break;
                case CommandNotFoundException _:
                    embed.Title = "Command Not Found";
                    embed.Description = $"No such command was found. Try any of the following to see the list of commands: {String.Join(", ", Config.Discord.CommandPrefixes.Select(x => $"`{x}help`"))}.";
                    break;
                case ChecksFailedException checksFailed:
                    List<string> failedChecks = new();
                    if (checksFailed.FailedChecks.Any(x => x is UserNotBannedAttribute))
                        return;
                    foreach (var attr in checksFailed.FailedChecks)
                    {
                        failedChecks.Add(ParseFailedCheck(attr));
                    }
                    embed.Title = "Command Prechecks Failed";
                    embed.Description = $"One or more command prechecks have failed:\n • {String.Join("\n • ", failedChecks)}";
                    break;
                case InvalidOperationException _:
                    if (e.Exception.Message.Equals("No matching subcommands were found, and this group is not executable."))
                    {
                        embed.Title = "Command Not Found";
                        embed.Description = $"No such command was found. Try any of the following to see the list of commands: {String.Join(", ", Config.Discord.CommandPrefixes.Select(x => $"`{x}help`"))}.";
                        break;
                    }
                    else
                        goto default;
                case NotImplementedException:
                    Log.Logger.Error(e.Exception, $"[COMMAND] {e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}");

                    embed.Title = "Error: Not Implemented";
                    embed.Description = $"This command is not fully implemented in it's current state and has led to an error.";
                    embed.AddField("Command Name", e.Command?.QualifiedName ?? "<unknown command>");
                    embed.WithFooter(DateTime.Now.ToString("HH:mm:ss"));
                    break;
                default:
                    Log.Logger.Error(e.Exception, $"[COMMAND] {e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}");

                    embed.Title = "Error";
                    embed.Description = $"Unknown error occured during command execution, please report this to the developer.";
                    embed.AddField("Exception Type", e.Exception.GetType().ToString());
                    embed.WithFooter(DateTime.Now.ToString("HH:mm:ss"));
                    break;
            }
            await e.Context.RespondAsync(embed: embed);
        }
        #endregion

        #region SlashCommandHandlers
        private Task SlashCommandInvoked(SlashCommandsExtension _, SlashCommandInvokedEventArgs e)
        {
            Log.Logger.Information($"[COMMAND] {e.Context.User.Username} invoked slash command '{e.Context.CommandName}'");
            return Task.CompletedTask;
        }
        private Task SlashCommandExecuted(SlashCommandsExtension _, SlashCommandExecutedEventArgs e)
        {
            Log.Logger.Information($"[COMMAND] {e.Context.User.Username} successfully executed slash command '{e.Context.CommandName}'");
            return Task.CompletedTask;
        }
        private async Task SlashCommandErrored(SlashCommandsExtension _, SlashCommandErrorEventArgs e)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(0xFF0000));
            switch (e.Exception)
            {
                case SlashExecutionChecksFailedException checksFailed:
                    List<string> failedChecks = new();
                    if(checksFailed.FailedChecks.Any(x => x is UserNotBannedSlashAttribute))
                    {
                        await e.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You cannot interact with the bot."));
                        return;
                    }
                    foreach (var attr in checksFailed.FailedChecks)
                    {
                        failedChecks.Add(ParseFailedSlashCheck(attr));
                    }
                    embed.Title = "Command Prechecks Failed";
                    embed.Description = $"One or more command prechecks have failed:\n • {String.Join("\n • ", failedChecks)}";
                    break;
                case DSharpPlus.Exceptions.BadRequestException badrq:
                    Log.Logger.Error(e.Exception, $"[COMMAND] {e.Context.User.Username} tried executing '{e.Context.CommandName}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}");
                    embed.WithTitle("Error: Bad Request");
                    embed.Description = $"Command interaction resulted in a bad request. This is likely an internal error.";
                    embed.AddField("Exception Type", e.Exception.GetType().ToString());
                    embed.WithFooter(DateTime.Now.ToString("HH:mm:ss"));
                    break;
                default:
                    Log.Logger.Error(e.Exception, $"[COMMAND] {e.Context.User.Username} tried executing '{e.Context.CommandName}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}");
                    embed.Title = "Error";
                    embed.Description = $"Error occured during command interaction, please report this to the developer.";
                    embed.AddField("Exception Type", e.Exception.GetType().ToString());
                    embed.WithFooter(DateTime.Now.ToString("HH:mm:ss"));
                    break;
            }
            await e.Context.Channel.SendMessageAsync(embed);
        }
        #endregion
        #endregion

        #region OffloadFunctions
        private static string ParseFailedCheck(CheckBaseAttribute attr)
        {
            return attr switch
            {
                CooldownAttribute _ => "Command is still under cooldown.",
                RequireOwnerAttribute _ => "Only the owner of the bot can use that command.",
                RequireBotPermissionsAttribute _ => "I dont have the required permissions for that.",
                RequireUserPermissionsAttribute _ => "You don't have permission to do that.",
                RequirePermissionsAttribute _ => "You don't have permission to do that.",
                RequireRolesAttribute _ => "You do not have a required role.",
                RequireNsfwAttribute _ => "This command can only be used in an NSFW channel.",
                RequireGuildAttribute _ => "This command can only be used in a guild",
                _ => $"Unknown required attribute: {attr.GetType()}"
            };
        }
        private static string ParseFailedSlashCheck(SlashCheckBaseAttribute attr)
        {
            return attr switch
            {
                SlashRequireOwnerAttribute _ => "Only the owner of the bot can use that command.",
                _ => $"Unknown required attribute: {attr.GetType()}"
            };
        }
        #endregion
    }
}
