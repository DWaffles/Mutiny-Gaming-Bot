using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MutinyBot.Common;
using MutinyBot.Database;
using MutinyBot.Modules;
using MutinyBot.Services;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MutinyBot
{
    public partial class MutinyBot
    {
        private MutinyBotConfig Config { get; }
        private IServiceProvider Services { get; }
        private DiscordClient Client { get; }
        private CommandsNextExtension Commands { get; }
        private SlashCommandsExtension SlashCommands { get; }
        public MutinyBot(MutinyBotConfig config)
        {
            Config = config;

            if (!FileHandler.VerifyConfig(Config))
                throw new ArgumentException("Config does not contain a valid token and/or prefix.");

            Log.Logger = (Config.Debug ? new LoggerConfiguration().MinimumLevel.Debug() : new LoggerConfiguration().MinimumLevel.Information())
                .WriteTo.Console()
                .WriteTo.File($"data{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}warning-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File($"data{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}info-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();

            Services = new ServiceCollection()
                .AddSingleton(this)
                .AddSingleton(Config)
                .AddSingleton<Random>()
                .AddSingleton<PetService>()
                .AddSingleton<UserService>()
                .AddSingleton<GuildService>()
                .AddSingleton<MemberService>()
                .AddSingleton<ImgurService>()
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

            SlashCommands = Client.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = Services
            });

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1),
                PollBehaviour = PollBehaviour.KeepEmojis,
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                ButtonBehavior = ButtonPaginationBehavior.Disable,

                ResponseBehavior = InteractionResponseBehavior.Ack,

                PaginationButtons = new PaginationButtons()
                {
                    SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, "first", "First"),
                    Left = new DiscordButtonComponent(ButtonStyle.Success, "left", "Left"),
                    Stop = new DiscordButtonComponent(ButtonStyle.Danger, "stop", "Stop"),
                    Right = new DiscordButtonComponent(ButtonStyle.Success, "right", "Right"),
                    SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, "last", "Last"),
                }
            });

            RegisterCommands();
            RegisterGatewayEvents();
            RegisterCommandEvents();
        }
        public async Task ConnectAsync()
        {
            var activity = new DiscordActivity(Config.Discord.BotStatus ?? $"[{string.Join(", ", Config.Discord.CommandPrefixes)}]help", ActivityType.Playing);
            try
            {
                await Client.ConnectAsync(activity);
            }
            catch (Exception ex) // SystemException
            {
                Log.Error(ex, "Exception occured while connecting to Discord.");
            }
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
        private void RegisterCommands()
        {
            Commands.SetHelpFormatter<CustomHelpFormatter>(); //Registering custom help formatter
            Commands.RegisterCommands(Assembly.GetExecutingAssembly()); //Registering commands from all modules

            //SlashCommands.RegisterCommands<SlashModule>(); //Clears slash commands globally
            //SlashCommands.RegisterCommands(typeof(SlashModule), /* ID */); //Clears slash commands per guild?

            if(Config.Discord.RegisterSlashCommands)
            {
                var modules = Assembly.GetExecutingAssembly().GetTypes() // Getting all types in the assembly
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SlashModule))).ToList(); // Selecting only the types that inherit from the base slash module

                if (!Config.Debug) //removing the debug/test slash module if it's not required
                    modules.Remove(typeof(DebugSlashModule));

                //if(String.IsNullOrEmpty(Config.Imgur.ClientId))
                    //modules.Remove(typeof());

                var guilds = Config.Discord.AuthorizedServerIds.ToList(); // list of all of the guilds to register slash commands for
                if (Config.Discord.MutinyGuildId != 0)
                    guilds.Add(Config.Discord.MutinyGuildId);

                Log.Logger.Information($"Registering ({String.Join(", ", modules.Select(type => type.Name))}) slash modules in following guilds: {String.Join(", ", Config.Discord.AuthorizedServerIds)}");
                foreach (Type type in modules)
                {
                    guilds.ForEach(guild => SlashCommands.RegisterCommands(type, guild));
                }
            }
        }
        private MutinyBot() { }
    }
}