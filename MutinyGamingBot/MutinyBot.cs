using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MutinyBot.Common;
using MutinyBot.Database;
using MutinyBot.Services;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MutinyBot
{
    public partial class MutinyBot
    {
        private MutinyBotConfig Config { get; }
        private IServiceProvider Services { get; }
        private DiscordClient Client { get; }
        private CommandsNextExtension Commands { get; }
        public MutinyBot(MutinyBotConfig config)
        {
            Config = config;

            if (!FileHandler.VerifyConfig(Config))
                throw new ArgumentException("Config does not contain a valid token and/or prefix.");

            var logger = (Config.Debug ? new LoggerConfiguration().MinimumLevel.Debug() : new LoggerConfiguration().MinimumLevel.Information())
                .WriteTo.Console()
                .WriteTo.File($"data{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();
            Log.Logger = logger;

            Services = new ServiceCollection()
                .AddSingleton(this)
                .AddSingleton<Random>()
                .AddSingleton<PetService>()
                .AddSingleton<UserService>()
                .AddSingleton<GuildService>()
                .AddSingleton<MemberService>()
                .AddDbContext<MutinyBotDbContext>()
                .BuildServiceProvider();

            ConfigureServices();

            var logFactory = new LoggerFactory().AddSerilog();
            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = Config.Discord.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LoggerFactory = logFactory,
                MinimumLogLevel = LogLevel.Trace,
                /*ReconnectIndefinitely = true,*/

                Intents = DiscordIntents.Guilds
                | DiscordIntents.GuildMessages
                | DiscordIntents.GuildMessageReactions
                | DiscordIntents.GuildMembers
                | DiscordIntents.DirectMessages
                | DiscordIntents.DirectMessageReactions
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = Config.Discord.CommandPrefixes,
                IgnoreExtraArguments = true,
                Services = Services
            });

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1),
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                ButtonBehavior = ButtonPaginationBehavior.Disable,
                PaginationButtons = new PaginationButtons()
                {
                    SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, "first", "First"/*, emoji: new DiscordComponentEmoji("⏮")*/),
                    Left = new DiscordButtonComponent(ButtonStyle.Success, "left", "Left"/*, emoji: new DiscordComponentEmoji("◀")*/),
                    Stop = new DiscordButtonComponent(ButtonStyle.Danger, "stop", "Stop"/*, emoji: new DiscordComponentEmoji("⏹")*/),
                    Right = new DiscordButtonComponent(ButtonStyle.Success, "right", "Right"/*, emoji: new DiscordComponentEmoji("▶")*/),
                    SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, "last", "Last"/*, emoji: new DiscordComponentEmoji("⏭")*/),
                }
            });

            RegisterEvents(); //Registering events

            Commands.SetHelpFormatter<MutinyBotHelpFormatter>(); //Registering custom help formatter
            Commands.RegisterCommands(Assembly.GetExecutingAssembly()); //Registering commands from all modules
        }
        public async Task ConnectAsync()
        {
            var activity = new DiscordActivity($"[{string.Join(", ", Config.Discord.CommandPrefixes)}]{(Config.Discord.BotStatus != null ? $" {Config.Discord.BotStatus}" : "help")}", ActivityType.Playing);
            await Client.ConnectAsync(activity);
            await Task.Delay(-1);
        }
        public DiscordColor GetBotColor()
        {
            return new DiscordColor(Config.HexCode);
        }
        public string[] GetCommandPrefixes()
        {
            return Config.Discord.CommandPrefixes;
        }
        private void ConfigureServices()
        {
            using var context = new MutinyBotDbContext();
            context.ApplyMigrations();

            /*var mediaService = (ImageService)Services.GetRequiredService(typeof(ImageService));
            mediaService.ConfigureMediaService(Path.Combine("data"));*/
        }
        private Task RunTaskAsync(Task task, [System.Runtime.CompilerServices.CallerMemberName] string parentCall = "")
        {
            Task.Run(async () =>
            {
                try
                {
                    await task;
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Exception occured in offloaded function from {parentCall}");
                }
            });
            return task;
        }
        private MutinyBot() { }
    }
}