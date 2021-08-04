using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Humanizer.Localisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class InfoModule : MutinyBotModule
    {
        private string DateTimeFormat { get; } = "ddd MMM dd, yyyy HH:mm tt";

        [Command("user"), Aliases("u"), RequireGuild]
        public async Task MemberInformationCommand(CommandContext ctx, DiscordMember member = null)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(embed: await GetMemberInfoEmbedAsync(ctx.Guild, member ?? ctx.Member));
        }
        [Command("role"), Aliases("r"), RequireGuild]
        public async Task RoleInformationCommand(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            await ctx.TriggerTypingAsync();
            var result = await GetRoleInfoEmbedAsync(ctx.Guild, role);
            if (result.Length == 1)
                await ctx.RespondAsync(embed: result[0].Embed);
            else
            {
                var interactivity = ctx.Client.GetInteractivity();
                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, result, buttons: null);
            }
        }
        [Command("guild"), Aliases("g"), RequireGuild]
        public async Task GuildInformationCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(embed: GetGuildInfoEmbed(ctx.Guild));
        }

        #region EmbedFunctions
        private async Task<DiscordEmbed> GetMemberInfoEmbedAsync(DiscordGuild guild, DiscordMember member)
        {
            var memberTask = MemberService.GetOrCreateMemberAsync(guild.Id, member.Id);
            string timeJoined = $"<t:{member.JoinedAt.ToUnixTimeSeconds()}:f>";
            string timeCreated = $"<t:{member.CreationTimestamp.ToUnixTimeSeconds()}:f>";

            string joinDuration = (DateTime.Now - member.JoinedAt).Humanize(3, maxUnit: TimeUnit.Year, minUnit: TimeUnit.Minute);
            string description = $"{member.Mention} joined {guild.Name} {joinDuration} ago.";

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{member.DisplayName}'s Information", iconUrl: $"{member.AvatarUrl}")
                .WithDescription(description)
                .AddField("Created", timeCreated, true)
                .AddField("Joined", timeJoined, true)
                .WithFooter("Times are localized via Discord.")
                .WithColor(GetBotColor());

            var memberModel = await memberTask;
            if (member.Roles.Any())
            {
                var orderedRoles = member.Roles.ToList().OrderByDescending(x => x.Position);
                var currentRoles = String.Join(", ", orderedRoles.Select(x => x.Mention));
                embed.AddField($"Current Roles ({member.Roles.Count()})", currentRoles);
            }
            if (memberModel.Guild.TrackMemberRoles && memberModel.RoleDictionary.Where(x => x.Value is false).Any())
            {
                var oldRoles = memberModel.RoleDictionary.Where(x => x.Value is false).Select(x => $"<@&{x.Key}>");
                embed.AddField($"Previous Roles ({oldRoles.Count()})", String.Join(", ", oldRoles));
            }

            return embed;
        }
        private async Task<Page[]> GetRoleInfoEmbedAsync(DiscordGuild guild, DiscordRole role)
        {
            var memberTask = guild.GetAllMembersAsync();
            var guildTask = GuildService.GetOrCreateGuildAsync(guild.Id, true);

            var maxCountPerPage = 20;
            var membersList = await memberTask;
            var guildModel = await guildTask;
            var currentHolders = membersList.Where(x => x.Roles.Contains(role)).Select(x => x.Mention).ToList();
            /*var previousHolders = guildModel.Members.Where(x => x.RoleDictionary.Keys.Contains(role.Id));*/

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{role.Name} Holders", iconUrl: $"{guild.IconUrl}")
                .WithFooter($"Guild Id: {guild.Id}; Role Id: {role.Id}")
                .WithColor(GetBotColor());

            if (currentHolders.Any() && currentHolders.Count <= maxCountPerPage) //standard embed
            {
                return new Page[]
                {
                    new Page {Embed = embed.AddField($"Current Holders ({currentHolders.Count})", string.Join("\n", currentHolders), true)}
                };
            }
            else if (currentHolders.Any()) //paginated embed
            {
                int numPages = (int)Math.Ceiling(currentHolders.Count * 1.0 / maxCountPerPage);
                var pages = new Page[numPages];
                var pageCurrentHolders = new List<string>();

                for (int pageIndex = 0; pageIndex < numPages; pageIndex++)
                {
                    pageCurrentHolders.Clear();
                    var pageEmbed = new DiscordEmbedBuilder(embed)
                        .WithFooter($"Page {pageIndex + 1}/{numPages}");

                    for (int currentHolderIndex = pageIndex * maxCountPerPage; currentHolderIndex < ((pageIndex + 1) * maxCountPerPage); currentHolderIndex++)
                    {
                        if (currentHolderIndex >= currentHolders.Count)
                        {
                            break;
                        }

                        pageCurrentHolders.Add($"{currentHolderIndex + 1}.) " + currentHolders[currentHolderIndex]);
                    }

                    if (pageCurrentHolders.Count != 0)
                    {
                        pageEmbed.AddField($"Current Holders ({currentHolders.Count})", string.Join("\n", pageCurrentHolders), true);
                    }

                    var page = new Page
                    {
                        Embed = pageEmbed.Build()
                    };
                    pages[pageIndex] = page;
                }

                return pages;
            }
            return new Page[]
            {
                new Page {Embed = embed.WithDescription($"There are no holders of {role.Mention}.")}
            };
        }
        private DiscordEmbed GetGuildInfoEmbed(DiscordGuild guild)
        {
            string dateTimeFormat = "ddd MMM dd, yyyy HH:mm tt";
            string dateTimeCreated = guild.CreationTimestamp.ToUniversalTime().ToString(dateTimeFormat);
            string joinDuration = (DateTime.Now - guild.CreationTimestamp).Humanize(3, maxUnit: TimeUnit.Year, minUnit: TimeUnit.Minute);
            string description = $"{guild.Name} was created {joinDuration}.";

            return new DiscordEmbedBuilder()
                .WithAuthor($"{guild.Name} Information", iconUrl: $"{guild.IconUrl}")
                .WithDescription(description)
                .AddField("Created", dateTimeCreated, true)
                .AddField("Members", guild.MemberCount.ToString(), true)
                .AddField("Roles", guild.Roles.Count.ToString(), true)
                .WithFooter("Times are in UTC using 24 hour time. The AM/PM modifier is for Americans.")
                .WithColor(GetBotColor());
        }
        #endregion
    }
}
