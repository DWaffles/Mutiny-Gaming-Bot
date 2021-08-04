using DSharpPlus;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace MutinyBot.Extensions
{
    public static class DiscordInteractionExtensions
    {
        /// <summary>
        /// Edits the original interaction response with the given content.
        /// </summary>
        /// <param name="interaction">The related interaction.</param>
        /// <param name="content">Replaces the current message content with this.</param>
        /// <returns>The edited <see cref="DiscordMessage"/>.</returns>
        public static async Task<DiscordMessage> EditOriginalResponseAsync(this DiscordInteraction interaction, string content)
        {
            return await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(content));
        }

        /// <summary>
        /// Creates a response to an interaction.
        /// </summary>
        /// <param name="interaction">The related interaction.</param>
        /// <param name="type">The type of response to create.</param>
        /// <param name="content">The content of the response.</param>
        public static Task CreateResponseAsync(this DiscordInteraction interaction, InteractionResponseType type, string content)
        {
            return interaction.CreateResponseAsync(type, new DiscordInteractionResponseBuilder().WithContent(content));
        }
    }
}
