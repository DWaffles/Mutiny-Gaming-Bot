﻿using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Humanizer;
using Humanizer.Localisation;
using MutinyBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class InformationSlashModule : SlashModule
    {
        [SlashCommand("user", "test")]
        public async Task MemberInformationCommand(InteractionContext ctx, [Option("member", "test")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            user = await ctx.Guild.GetMemberAsync(user.Id);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(await GetMemberInfoEmbedAsync(ctx.Guild, user as DiscordMember)));
        }
        [SlashCommand("role", "See the list of users with this role.")]
        public async Task RoleInformationCommand(InteractionContext ctx, [Option("role", "Role to search users for.")] DiscordRole role)
        {
            var result = await GetRoleInfoEmbedAsync(ctx.Guild, role);
            if (result.Length == 1)
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(result[0].Embed));
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.Pong);
                var interactivity = ctx.Client.GetInteractivity();
                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, result, buttons: null);
            }
        }
    }
}