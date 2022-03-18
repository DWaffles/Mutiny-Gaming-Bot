using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MutinyBot.Enums;
using MutinyBot.Extensions;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public class CreatorPetModule : CreatorModule
    {
        [Command("addpetowner"), Aliases("apo")]
        [Description("Makes a user a verified pet owner and able to add images to the pet list for all guilds.")]
        public async Task AddPetOwner(CommandContext ctx, DiscordUser user)
        {
            await ctx.TriggerTypingAsync();

            var userModel = await UserService.GetOrCreateUserAsync(user.Id);
            userModel.IsPetOwner = true;
            await UserService.UpdateUserAsync(userModel);

            var message = new DiscordMessageBuilder()
                .WithAllowedMentions(Mentions.None)
                .WithContent($"{user.Mention} can now add to the pet list.");
            await ctx.RespondAsync(message);
        }
        [Command("removepetowner"), Aliases("rpo")]
        public async Task RemovePetOwner(CommandContext ctx, DiscordUser user)
        {
            await ctx.TriggerTypingAsync();

            var userModel = await UserService.GetOrCreateUserAsync(user.Id);
            userModel.IsPetOwner = false;
            await UserService.UpdateUserAsync(userModel);

            var message = new DiscordMessageBuilder()
                .WithAllowedMentions(Mentions.None)
                .WithContent($"{user.Mention} can no longer add to the pet list.");
            await ctx.RespondAsync(message);
        }
    }
}
