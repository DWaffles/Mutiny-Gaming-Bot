using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireGuild()]
    public class RoleCommands : MutinyBotModule
    {
        [Command("role"), Aliases("r")]
        public async Task RoleInformation(CommandContext ctx, DiscordRole discordRole)
        {
            Console.WriteLine("ROLE STRING");
            string authorName = "Role Information";
            string authorUrl = $"{ctx.Guild.IconUrl}";

            await ctx.TriggerTypingAsync();

            var memberList = ctx.Guild.Members.Values;
            if (memberList.Count() != ctx.Guild.MemberCount)
            {
                authorName += " (Retrieved)";
                memberList = await ctx.Guild.GetAllMembersAsync();
            }

            var currentHolders = memberList.Where(x => x.Roles.Contains(discordRole)).Select(x => x.Mention).ToList();

            var embed = new DiscordEmbedBuilder()
                        .WithAuthor(authorName, iconUrl: authorUrl)
                        .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

            int maxCount = 20;

            if (currentHolders.Count > 0)
            {
                if(currentHolders.Count <= maxCount)
                {
                    embed.AddField($"Current Holders ({currentHolders.Count})", string.Join("\n", currentHolders), true);
                    await ctx.RespondAsync(embed: embed);
                }
                else
                {
                    int numPages;
                    List<string> pageCurrentHolders = new List<string>();
                    var interactivity = ctx.Client.GetInteractivity();

                    numPages = (int)Math.Ceiling(currentHolders.Count * 1.0 / maxCount);

                    Page[] pages = new Page[numPages];

                    for (int pageIndex = 0; pageIndex < numPages; pageIndex++)
                    {
                        pageCurrentHolders.Clear();

                        var pageEmbed = new DiscordEmbedBuilder()
                            .WithAuthor(authorName, iconUrl: authorUrl)
                            .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

                        pageEmbed.WithFooter($"Page {pageIndex + 1}/{numPages}");

                        for (int currentHolderIndex = pageIndex * maxCount; currentHolderIndex < ((pageIndex + 1) * maxCount); currentHolderIndex++)
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
                    await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, timeoutoverride: TimeSpan.FromMinutes(2));
                }
            }
            /*if (previousHolderIds.Count > 0)
            {
                embed.AddField($"Previous Holders ({previousHolderIds.Count})", string.Join("\n", previousHolderIds), true);
            }*/
            
        }
        [Command("role")]
        public async Task RoleInformation(CommandContext ctx, [RemainingText] string roleName)
        {
            Console.WriteLine("ROLE STRING");
            var foundRole = ctx.Guild.Roles.Values.FirstOrDefault(role => role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (foundRole != null)
            {
                await RoleInformation(ctx, foundRole);
            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Role Not Found",
                    Description = $"No such role with that name was found.",
                    Color = new DiscordColor(0xFF0000) // red
                };

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}