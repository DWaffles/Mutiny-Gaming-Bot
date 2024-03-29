﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MutinyBot.Models;
using MutinyBot.Services;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot
{
    public partial class MutinyBot
    {
        private void RegisterGatewayEvents()
        {
            Client.SessionCreated += OnSessionCreated;
            Client.SessionResumed += OnSessionResumed;
            Client.ClientErrored += OnClientError;

            Client.MessageCreated += Client_MessageCreated;

            Client.GuildDownloadCompleted += OnGuildsCompleted;
            Client.GuildAvailable += OnGuildAvailable;
            Client.GuildCreated += OnGuildJoined;

            Client.GuildMemberUpdated += OnGuildMemberUpdated;
            Client.GuildMemberAdded += OnGuildMemberAdded;
            Client.GuildMemberRemoved += OnGuildMemberRemoved;
        }

        #region EventHandlers
        private Task OnSessionCreated(DiscordClient client, SessionReadyEventArgs e)
        {
            Log.Logger.Information("[CLIENT] Client is ready to process events.");
            return Task.CompletedTask;
        }
        private Task OnSessionResumed(DiscordClient client, SessionReadyEventArgs e)
        {
            Log.Logger.Information("[CLIENT] Client is ready to process events.");
            return Task.CompletedTask;
        }
        private Task OnClientError(DiscordClient client, ClientErrorEventArgs e)
        {
            Log.Logger.Error(e.Exception, "[CLIENT] Exception occured.");
            return Task.CompletedTask;
        }
        private async Task Client_MessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            if (Config.Debug)
                Log.Debug("[CLIENT] DirectMessage Recieved.");
            if (e.Guild != null)
                await MemberMessageAsync(e.Guild, e.Message);
        }
        private Task OnGuildsCompleted(DiscordClient client, GuildDownloadCompletedEventArgs e)
        {
            Log.Logger.Information("[GUILD] Guild download completed.");
            return Task.CompletedTask;
        }
        private async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            Log.Logger.Information($"[GUILD] Guild available: {e.Guild.Name}. ID: {e.Guild.Id}.");
            await CreateOrUpdateGuildAsync(e.Guild);
        }
        private async Task OnGuildJoined(DiscordClient client, GuildCreateEventArgs e)
        {
            Log.Logger.Information($"[GUILD] Guild joined: {e.Guild.Name}. ID: {e.Guild.Id}.");
            await CreateOrUpdateGuildAsync(e.Guild);
        }
        private async Task OnGuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs e)
        {
            Log.Logger.Information($"[MEMBER] Member updated in: {e.Guild.Name}. ID: {e.Guild.Id}");
            await MemberUpdatedAsync(e.Member);
        }
        private async Task OnGuildMemberAdded(DiscordClient client, GuildMemberAddEventArgs e)
        {
            Log.Logger.Information($"[MEMBER] Member joined in: {e.Guild.Name}. ID: {e.Guild.Id}");
            await MemberJoinedAsync(e.Guild, e.Member);
        }
        private async Task OnGuildMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs e)
        {
            Log.Logger.Information($"[MEMBER] Member removed in: {e.Guild.Name}. ID: {e.Guild.Id}");
            await MemberRemovedAsync(e.Member);
        }
        #endregion

        #region OffloadFunctions
        private async Task CreateOrUpdateGuildAsync(DiscordGuild guild)
        {
            Log.Debug($"[CLIENT] Updating db entry for: {guild.Name} ({guild.Id})");

            var guildService = (GuildService)Services.GetRequiredService(typeof(GuildService));

            var guildTask = guildService.GetOrCreateGuildAsync(guild);
            var memberTask = guild.GetAllMembersAsync();

            await Task.WhenAll(guildTask, memberTask);

            var dbGuild = await guildTask;
            dbGuild.UpdateGuild(guild, await memberTask);

            await guildService.UpdateGuildAsync(dbGuild);
        }
        private async Task MemberUpdatedAsync(DiscordMember member)
        {
            var memberService = (MemberService)Services.GetRequiredService(typeof(MemberService));
            var entity = await memberService.GetOrCreateMemberAsync(member);
            entity.UpdateMember(member, entity.Guild.TrackMemberRoles);
            await memberService.UpdateMemberAsync(entity);
        }
        private async Task MemberMessageAsync(DiscordGuild guild, DiscordMessage message)
        {
            var memberService = (MemberService)Services.GetRequiredService(typeof(MemberService));
            var entity = await memberService.GetOrCreateMemberAsync(guild.Id, message.Author.Id);
            entity.LastMessageTimestamp = entity.Guild.TrackMessageTimestamps ? message.CreationTimestamp : DateTimeOffset.FromUnixTimeSeconds(0);
            await memberService.UpdateMemberAsync(entity);
        }
        private async Task MemberJoinedAsync(DiscordGuild guild, DiscordMember member)
        {
            var memberService = (MemberService)Services.GetRequiredService(typeof(MemberService));
            var entity = await memberService.GetOrCreateMemberAsync(guild.Id, member.Id);

            await AnnounceMember(guild, member, entity); //announce new join

            entity.UpdateMember(member, entity.Guild.TrackMemberRoles);
            await memberService.UpdateMemberAsync(entity);
        }
        private async Task MemberRemovedAsync(DiscordMember member)
        {
            var memberService = (MemberService)Services.GetRequiredService(typeof(MemberService));
            var entity = await memberService.GetOrCreateMemberAsync(member);
            entity.CurrentMember = false;
            await memberService.UpdateMemberAsync(entity);
        }
        private async Task AnnounceMember(DiscordGuild guild, DiscordMember member, MemberModel dbMember)
        {
            if (dbMember.Guild.JoinLogChannelId == 0)
                return;

            var channel = guild.GetChannel(dbMember.Guild.JoinLogChannelId);
            if (channel is null) //joinlog channel has been deleted or something
            {
                dbMember.Guild.JoinLogChannelId = 0;
                return;
            }

            /*string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";*/
            string timeCreated = $"<t:{member.CreationTimestamp.ToUnixTimeSeconds()}:R>";
            string timeJoined = $"<t:{member.JoinedAt.ToUnixTimeSeconds()}:R>";
            string description = dbMember.CurrentMember ? $"{member.Mention} has not joined before." : $"{member.Mention} has joined {dbMember.TimesJoined} times before.";

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"New Member: {member.DisplayName}", iconUrl: $"{member.AvatarUrl}")
                .WithDescription(description)
                .AddField("Created", timeCreated, true)
                .AddField("Joined", timeJoined, true)
                .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                .WithColor(GetBotColor());

            var previousRoles = dbMember.RoleDictionary.Where(x => x.Value == true).Select(x => "<@&" + x.Key + ">"); //Previous User Roles
            var pastRoles = dbMember.RoleDictionary.Where(x => x.Value == false).Select(x => "<@&" + x.Key + ">"); //All Past Roles

            if (previousRoles.Any())
            {
                embed.AddField($"Previous Roles ({previousRoles.Count()})", String.Join(", ", previousRoles));
            }
            if (pastRoles.Any())
            {
                embed.AddField($"All Past Roles ({pastRoles.Count()})", String.Join(", ", pastRoles));
            }

            await channel.SendMessageAsync(embed: embed);
        }
        #endregion
    }
}
