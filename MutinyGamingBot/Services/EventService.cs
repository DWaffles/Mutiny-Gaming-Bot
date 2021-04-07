using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MutinyBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    public interface IEventService
    {
        public void Initialize();
    }
    public class EventService : IEventService
    {
        public MutinyBot MutinyBot { protected get; set; }
        public EventService(MutinyBot mutinyBot)
        {
            MutinyBot = mutinyBot;
            Console.WriteLine("EVENTSERVICE");
        }
        public void Initialize()
        {
            if (MutinyBot.Config.Debug)
                Console.WriteLine("EVENTSERVICE INIT");
            MutinyBot.Client.Ready += OnReady;
            MutinyBot.Client.ClientErrored += OnClientError;
            MutinyBot.Client.GuildAvailable += OnGuildAvailable;
            MutinyBot.Client.GuildCreated += OnGuildJoined;
            MutinyBot.Client.GuildMemberUpdated += OnGuildMemberUpdated;
            MutinyBot.Client.GuildMemberAdded += OnGuildMemberAdded;
            MutinyBot.Client.GuildMemberRemoved += OnGuildMemberRemoved;
            MutinyBot.Client.MessageCreated += Client_MessageCreated;
        }
        private Task OnReady(DiscordClient client, ReadyEventArgs e)
        {
            client.Logger.LogInformation(new EventId(0, "Ready"), "Client is ready to process events.");
            return Task.CompletedTask;
        }
        private Task OnClientError(DiscordClient client, ClientErrorEventArgs e)
        {
            client.Logger.LogError(new EventId(1, "Error"), e.Exception, "Exception occured");
            return Task.CompletedTask;
        }
        private Task Client_MessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            return Task.CompletedTask;
        }
        private async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            //UpdateGuild(e.Guild);
            await UpdateGuild(e.Guild);
            //await RunTaskAsync(UpdateGuild(e.Guild));

            client.Logger.LogInformation(new EventId(0, "GuildReady"), $"Guild available: {e.Guild.Name}. ID: {e.Guild.Id}");
        }
        private async Task OnGuildJoined(DiscordClient client, GuildCreateEventArgs e)
        {
            await UpdateGuild(e.Guild);

            client.Logger.LogInformation(new EventId(0, "GuildJoined"), $"Guild joined: {e.Guild.Name}. ID: {e.Guild.Id}");
        }
        private async Task OnGuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs e)
        {
            await UpdateMember(e.Member);

            client.Logger.LogInformation(new EventId(0, "MemberUpdated"), $"Member updated in: {e.Guild.Name}. ID: {e.Guild.Id}");
        }
        private Task OnGuildMemberAdded(DiscordClient client, GuildMemberAddEventArgs e)
        {
            RunTaskAsync(NewMember(e.Guild, e.Member));

            client.Logger.LogInformation(new EventId(0, "MemberAdded"), $"Member joined in: {e.Guild.Name}. ID: {e.Guild.Id}");
            return Task.CompletedTask;
        }
        private Task OnGuildMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs e)
        {
            return Task.CompletedTask;
        }

        //
        public Task RunTaskAsync(Task task)
            => RunTaskAsync(() => task);
        public Task RunTaskAsync(Func<Task> func)
        {
            Task.Run(async () =>
            {
                try
                {
                    await func.Invoke();
                }
                catch (Exception)
                {
                    // Log Exception
                }
            });

            return Task.CompletedTask;
        }
        private async Task UpdateGuild(DiscordGuild guild)
        {
            IGuildService guildService = MutinyBot.Services.GetService<IGuildService>();
            IMemberService memberService = MutinyBot.Services.GetService<IMemberService>();

            await guildService.GetOrCreateGuildAsync(guild.Id);

            var memberList = await guild.GetAllMembersAsync();

            if (MutinyBot.Config.Debug)
            {
                Console.WriteLine($"Updating {guild.Name}, ID: {guild.Id}, MemberCount: {guild.MemberCount}");
            }

            foreach (DiscordMember member in memberList)
            {
                MemberEntity memberEntity = await memberService.GetOrCreateMemberAsync(member);

                Dictionary<ulong, bool> tmpDict = new Dictionary<ulong, bool>();
                foreach (var role in member.Roles)
                {
                    tmpDict.Add(role.Id, true);
                }
                if (tmpDict.Count == memberEntity.RoleDictionary.Count && !tmpDict.Except(memberEntity.RoleDictionary).Any())
                {
                    if (MutinyBot.Config.Debug)
                        Console.WriteLine("NO CHANGE");
                }
                else
                {
                    if (MutinyBot.Config.Debug)
                        Console.WriteLine("CHANGED");
                    memberEntity.UpdateEntity(member);
                    await memberService.UpdateMemberAsync(memberEntity);
                }
            }
            if (MutinyBot.Config.Debug)
                Console.WriteLine($"END UPDATE");
        }
        private async Task UpdateMember(DiscordMember member)
        {
            IMemberService memberService = MutinyBot.Services.GetService<IMemberService>();
            MemberEntity memberEntity = await memberService.GetOrCreateMemberAsync(member);

            Dictionary<ulong, bool> tmpDict = new Dictionary<ulong, bool>();
            foreach (var role in member.Roles)
            {
                tmpDict.Add(role.Id, true);
            }
            if (tmpDict.Count == memberEntity.RoleDictionary.Count && !tmpDict.Except(memberEntity.RoleDictionary).Any())
            { }
            else
            {
                memberEntity.UpdateEntity(member);
                await memberService.UpdateMemberAsync(memberEntity);
            }
        }
        private async Task NewMember(DiscordGuild guild, DiscordMember member)
        {
            IGuildService guildService = MutinyBot.Services.GetService<IGuildService>();
            GuildEntity guildEntity = await guildService.GetOrCreateGuildAsync(guild.Id);
            IMemberService memberService = MutinyBot.Services.GetService<IMemberService>();
            MemberEntity memberEntity = await memberService.GetNewMemberJoinedAsync(member);

            if (guildEntity.JoinLogEnabled && (guildEntity.JoinLogChannelId != 0))
            {
                var channel = guild.GetChannel(guildEntity.JoinLogChannelId);
                if (channel != null)
                {
                    string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";
                    string dateTimeJoined = member.JoinedAt.ToUniversalTime().ToString(dateTimeFormat);
                    string dateTimeCreated = member.CreationTimestamp.ToUniversalTime().ToString(dateTimeFormat);
                    string description;
                    if ((memberEntity.TimesJoined - 1) == 0)
                        description = $"{member.Mention} has not joined before.";
                    else
                        description = $"{member.Mention} has joined {memberEntity.TimesJoined - 1} times before.";

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor($"New Member: {member.DisplayName}", iconUrl: $"{member.AvatarUrl}")
                        .WithDescription(description)
                        .AddField("Created", dateTimeCreated, true)
                        .AddField("Joined", dateTimeJoined, true)
                        .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                        .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

                    var previousRoles = memberEntity.RoleDictionary.Where(x => x.Value == true).Select(x => "<@&" + x.Key + ">"); //Previous User Roles
                    var pastRoles = memberEntity.RoleDictionary.Where(x => x.Value == false).Select(x => "<@&" + x.Key + ">"); //All Past Roles

                    if (previousRoles.Count() > 0)
                    {
                        embed.AddField($"Previous Roles ({previousRoles.Count()})", String.Join(", ", previousRoles));
                    }
                    if (pastRoles.Count() > 0)
                    {
                        embed.AddField($"All Past Roles ({pastRoles.Count()})", String.Join(", ", pastRoles));
                    }

                    await channel.SendMessageAsync(embed: embed);
                }
                else
                {
                    guildEntity.JoinLogChannelId = 0;
                    await guildService.UpdateGuildAsync(guildEntity);
                }
            }
        }
    }
}
