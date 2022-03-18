using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [SlashRequireOwner]
    internal class DebugSlashModule : SlashModule
    {
        [SlashCommand("ack", "Ack.")]
        public async Task AckCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Ack.");
        }
        [SlashCommand("tag", "Test")]
        public async Task TagAttachmentCommand(InteractionContext ctx,
            [Option("image", "Image types: PNG, JPEG, WebP, GIF")] DiscordAttachment attachment,
            [Option("title", "test")] string title,
            [Option("tags", "Seperate each tag by a space.")] string tags,
            [Option("description", "test")] string description = null)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle(title ?? "Null")
                .WithColor(GetBotColor())
                .WithDescription(description ?? "N/A")
                .AddField("Tags", tags ?? "Null");

            await ctx.CreateResponseAsync(embed);
        }
        [SlashCommand("modal", "Modal")]
        public async Task ModalCommand(InteractionContext ctx,
            [Option("image", "Image types: PNG, JPEG, WebP, GIF")] DiscordAttachment attachment = null)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("TEST")
                .WithCustomId($"id-modal-{ctx.User.Id}")
                .AddComponents(new TextInputComponent(label: "Title of Tag", customId: "id-title", placeholder: "Enter title...", max_length: 32))
                .AddComponents(new TextInputComponent("Tags", "id-tags", null, required: true, style: TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Description?", "id-description", "Enter description...", required: false, style: TextInputStyle.Paragraph));
            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);

            var response = await ctx.Client.GetInteractivity().WaitForModalAsync($"id-modal-{ctx.User.Id}", user: ctx.User);
            await response.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await response.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{String.Join("\n", response.Result.Values.Values)}"));
        }
    }
}
