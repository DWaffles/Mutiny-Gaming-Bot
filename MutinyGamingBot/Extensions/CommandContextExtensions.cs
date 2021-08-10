using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using MutinyBot.Enums;
using System;
using System.Threading.Tasks;

namespace MutinyBot.Extensions
{
    public static class CommandContextExtensions
    {
        private static string ConfirmId { get; } = "confirm_id";
        private static string DenyId { get; } = "deny_id";
        private static DiscordButtonComponent[] Buttons { get; } = new DiscordButtonComponent[] { new(ButtonStyle.Success, ConfirmId, "Yes"), new(ButtonStyle.Danger, DenyId, "No") };
        private static DiscordButtonComponent[] DisabledButtons { get; } = new DiscordButtonComponent[] { new(ButtonStyle.Success, ConfirmId, "Yes", true), new(ButtonStyle.Danger, DenyId, "No", true) };

        /// <summary>
        /// Asks the user tied to the current context to confirm an action.
        /// </summary>
        /// <param name="ctx">The CommandContext to wait on.</param>
        /// <param name="content">The action or query to ask the user.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in DSharpPlus.Interactivity.InteractivityConfiguration.Timeout.</param>
        /// <returns>A tuple containing the user response as <see cref="ConfirmationResult"/> and the <see cref="DiscordInteraction"/>.</returns>
        public static async Task<(ConfirmationResult UserResponse, DiscordInteraction Interaction)> WaitForConfirmationInteraction(this CommandContext ctx, string content, bool deleteButtonsAfter = false, TimeSpan? timeoutOverride = null)
        {
            var builder = new DiscordMessageBuilder()
                .WithContent(content)
                .AddComponents(Buttons);

            var message = await ctx.RespondAsync(builder);
            builder.Clear();
            builder.WithContent(content);
            if (!deleteButtonsAfter)
                builder.AddComponents(DisabledButtons);

            var interactivityResult = await message.WaitForButtonAsync(user: ctx.User, timeoutOverride);
            await message.ModifyAsync(builder);

            if (interactivityResult.TimedOut)
            {
                return (ConfirmationResult.TimedOut, null);
            }
            else
            {
                var buttonPress = interactivityResult.Result;
                if (buttonPress.Id == DenyId)
                {
                    return (ConfirmationResult.Denied, buttonPress.Interaction);
                }
                else
                {
                    return (ConfirmationResult.Confirmed, buttonPress.Interaction);
                }
            }
        }

        /// <summary>
        /// Asks the user tied to the current context to confirm an action.
        /// </summary>
        /// <param name="ctx">The CommandContext to wait on.</param>
        /// <param name="embed">An embed containing the action or query to ask the user.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in DSharpPlus.Interactivity.InteractivityConfiguration.Timeout.</param>
        /// <returns>A tuple containing the user response as <see cref="ConfirmationResult"/> and the <see cref="DiscordInteraction"/>.</returns>
        public static async Task<(ConfirmationResult UserResponse, DiscordInteraction Interaction)> WaitForConfirmationInteraction(this CommandContext ctx, DiscordEmbed embed, bool deleteButtonsAfter = false, TimeSpan? timeoutOverride = null)
        {
            var builder = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(Buttons);

            var message = await ctx.RespondAsync(builder);
            builder.Clear();
            builder.AddEmbed(embed);
            if (!deleteButtonsAfter)
                builder.AddComponents(DisabledButtons);

            var interactivityResult = await message.WaitForButtonAsync(user: ctx.User, timeoutOverride);
            await message.ModifyAsync(builder);

            if (interactivityResult.TimedOut)
            {
                return (ConfirmationResult.TimedOut, null);
            }
            else
            {
                var buttonPress = interactivityResult.Result;
                if (buttonPress.Id == DenyId)
                {
                    return (ConfirmationResult.Denied, buttonPress.Interaction);
                }
                else
                {
                    return (ConfirmationResult.Confirmed, buttonPress.Interaction);
                }
            }
        }
    }
}
