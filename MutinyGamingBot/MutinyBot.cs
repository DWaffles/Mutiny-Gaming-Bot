using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MutinyBot.Common;
using MutinyBot.Database;
using MutinyBot.Modules.Attributes;
using MutinyBot.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MutinyBot
{
    public class MutinyBot
    {
        public Configuration Config { get; }
        public DiscordClient Client { get; }
        public IServiceProvider Services { get; }
        private EventId BotEventId { get; } = new EventId(0, "Ready");
        public MutinyBot()
        {
            Config = new Configuration();
            if (!Config.ReadConfig())
                return;

            Services = new ServiceCollection()
                .AddSingleton(this)
                .AddSingleton(Config)
                .AddSingleton<Random>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IGuildService, GuildService>()
                .AddSingleton<IEventService, EventService>()
                .AddSingleton<IMemberService, MemberService>()
                .AddDbContext<MutinyBotDbContext>()
                .BuildServiceProvider();

            var logLevel = LogLevel.Information;
            if (Config.Debug)
                logLevel = LogLevel.Debug;

            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                MinimumLogLevel = logLevel,

                Intents = DiscordIntents.Guilds
                | DiscordIntents.GuildMessages
                | DiscordIntents.GuildMessageReactions
                | DiscordIntents.GuildMembers
                | DiscordIntents.GuildBans
                | DiscordIntents.GuildEmojis
                | DiscordIntents.DirectMessages
                | DiscordIntents.DirectMessageReactions
                | DiscordIntents.AllUnprivileged
            });

            CommandsNextExtension commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = Config.CommandPrefixes,
                IgnoreExtraArguments = true,
                Services = Services
            });

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                Timeout = TimeSpan.FromMinutes(1)
            });

            //Registering events
            commands.CommandExecuted += CommandExecuted;
            commands.CommandErrored += CommandErrored;

            //Registering Commands
            commands.RegisterCommands(Assembly.GetExecutingAssembly()); //register commands from all modules

            InitializeServices();


        }
        public async Task ConnectAsync()
        {
            var activity = new DiscordActivity($"[{string.Join(", ", Config.CommandPrefixes)}] Farming Simulator 2019", ActivityType.Playing);
            await Client.ConnectAsync(activity);
            await Task.Delay(-1);
        }
        private void InitializeServices()
        {
            IEventService events = Services.GetService<IEventService>();
            events.Initialize();
        }
        private Task CommandExecuted(CommandsNextExtension _, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");
            return Task.CompletedTask;
        }
        private async Task CommandErrored(CommandsNextExtension _, CommandErrorEventArgs e)
        {
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(0xFF0000));

            switch (e.Exception)
            {
                case ArgumentException _:
                    embed.Title = "Invalid arguments given";
                    embed.Description = $"The arguments given for the command is not valid.";
                    break;
                case CommandNotFoundException _:
                    embed.Title = "Command not found";
                    embed.Description = $"No such command was found.";
                    break;
                case ChecksFailedException _:
                    List<string> failedChecks = new();
                    foreach (var attr in ((ChecksFailedException)e.Exception).FailedChecks)
                    {
                        if (attr is UserNotBannedAttribute)
                            return;
                        else
                            failedChecks.Add(ParseFailedCheck(attr));
                    }
                    embed.Title = "Command Prechecks Failed";
                    embed.Description = $"One or more command prechecks have failed:\n • {String.Join("\n • ", failedChecks)}";
                    break;
                default:
                    embed.Title = "Unknown Error Occured";
                    embed.Description = $"Unknown error occured, please report this to the developer. \nException Type: {e.Exception.GetType()}";
                    break;
            }
            await e.Context.RespondAsync("", embed: embed);
        }
        private static string ParseFailedCheck(CheckBaseAttribute attr)
        {
            return attr switch
            {
                //UserNotBannedAttribute _ => "You have been banned from interacted with the bot.",
                CooldownAttribute _ => "Command is still under cooldown.",
                RequireOwnerAttribute _ => "Only the owner of the bot can use that command.",
                RequireBotPermissionsAttribute _ => "I dont have the required permissions for that.",
                RequirePermissionsAttribute _ => "You don't have permission to do that.",
                RequireRolesAttribute _ => "You do not have a required role.",
                RequireUserPermissionsAttribute _ => "You don't have permission to do that.",
                RequireNsfwAttribute _ => "This command can only be used in an NSFW channel.",
                RequireGuildAttribute _ => "This command can only be used in a guild",
                _ => "Unknown required attribute."
            };
        }
        // Permission checks?
    }
}