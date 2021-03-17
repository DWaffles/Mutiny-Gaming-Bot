using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireGuild()]
    public class RoleCommands : MutinyBotModule
    {
        private DiscordEmbed GetOutDatedErrorEmbed()
        {
            return new DiscordEmbedBuilder
            {
                Title = "Outdated Command",
                Description = $"The `user info` command has been depreciated. The command is now just `user`.",
                Color = new DiscordColor(0xFF0000) // red
            };
        }
        private DiscordEmbed GetOutDatedWarningEmbed()
        {
            return new DiscordEmbedBuilder
            {
                Title = "Outdated Command",
                Description = $"After March 21st, 2021 the `user info` command will be depreciated. Please use `user` instead.",
                Color = new DiscordColor(0xFF0000) // red
            };
        }
        [Command("role")]
        public async Task UserInformation(CommandContext ctx, string sub, DiscordRole discordRole)
        {
            if (DateTime.Now > new DateTime(2021, 3, 21))
            {
                await ctx.RespondAsync(embed: GetOutDatedErrorEmbed());
                return;
            }
            else
            {
                await ctx.RespondAsync(embed: GetOutDatedWarningEmbed());
            }
            if (sub.Equals("info", StringComparison.OrdinalIgnoreCase))
            {
                await RoleInformation(ctx, discordRole);
            }
            else if (sub.Equals("i", StringComparison.OrdinalIgnoreCase))
            {
                await RoleInformation(ctx, discordRole);
            }
        }
        [Command("role")]
        public async Task UserInformation(CommandContext ctx, string sub, [RemainingText] string roleName)
        {
            if (DateTime.Now > new DateTime(2021, 3, 21))
            {
                await ctx.RespondAsync(embed: GetOutDatedErrorEmbed());
                return;
            }
            if (sub.Equals("info", StringComparison.OrdinalIgnoreCase))
            {
                await RoleInformation(ctx, roleName);
            }
            else if (sub.Equals("i", StringComparison.OrdinalIgnoreCase))
            {
                await RoleInformation(ctx, roleName);
            }
        }
        [Command("role"), Aliases("r")]
        public async Task RoleInformation(CommandContext ctx, DiscordRole discordRole)
        {
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

            if (currentHolders.Count > 0)
            {
                embed.AddField($"Current Holders ({currentHolders.Count})", string.Join("\n", currentHolders), true);
            }
            /*if (previousHolderIds.Count > 0)
            {
                embed.AddField($"Previous Holders ({previousHolderIds.Count})", string.Join("\n", previousHolderIds), true);
            }*/
            await ctx.RespondAsync(embed: embed);
        }
        [Command("role")]
        public async Task RoleInformation(CommandContext ctx, [RemainingText] string roleName)
        {
            var foundRole = ctx.Guild.Roles.Values.FirstOrDefault(role => role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (foundRole != null)
            {
                await RoleInformation(ctx, foundRole);
            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "User Not Found",
                    Description = $"No such user with that nickname or username was found.",
                    Color = new DiscordColor(0xFF0000) // red
                };

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
