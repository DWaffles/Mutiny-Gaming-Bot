﻿using DSharpPlus;
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
            Config.ReadConfig();
            Config.VerifyConfig();

            Services = new ServiceCollection()
                .AddSingleton(this)
                .AddSingleton(Config)
                .AddSingleton<Random>()
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

            Client.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");

            };

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

            InitializeServices();

            //Registering Commands
            commands.RegisterCommands(Assembly.GetExecutingAssembly()); //register commands from all modules
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
                    List<string> failedChecks = new List<string>();
                    foreach (var attr in ((ChecksFailedException)e.Exception).Command.ExecutionChecks)
                    {
                        failedChecks.Add(ParseFailedCheck(attr));
                    }
                    embed.Title = "Access Denied";
                    embed.Description = $"You do not have the required permissions required to execute this command.\n • {String.Join("\n • ", failedChecks.ToArray())}";
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
                CooldownAttribute _ => "Command is still under cooldown.",
                RequireOwnerAttribute _ => "Only the owner of the bot can use that command.",
                RequirePermissionsAttribute _ => "You don't have permission to do that.",
                RequireRolesAttribute _ => "You do not have a required role.",
                RequireUserPermissionsAttribute _ => "You don't have permission to do that.",
                RequireNsfwAttribute _ => "This command can only be used in an NSFW channel.",
                RequireGuildAttribute _ => "This command can only be used in a guild",
                _ => "Unknown required attribute."
            };
        }
    }
}