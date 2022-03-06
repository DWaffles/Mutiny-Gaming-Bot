using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [SlashRequireOwner]
    internal class CreatorSlashModule : SlashModule
    {
        [SlashCommand("attachment", "Test.")]
        public async Task AttachmentCommand(InteractionContext ctx, [Option("image", "Image types: PNG, JPEG, WebP, GIF")] DiscordAttachment attachment)
        {
            await ctx.CreateResponseAsync(String.Join("\n", $"attachment: {attachment.FileName}", $"attachment: <{attachment.Url}>", $"attachment: {attachment.FileSize}"));
        }
    }
}
