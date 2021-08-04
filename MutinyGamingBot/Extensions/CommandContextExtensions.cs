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

        /// <summary>
        /// Asks the user tied to the current context to confirm an action.
        /// </summary>
        /// <param name="ctx">The CommandContext to wait on.</param>
        /// <param name="content">The action or query to ask the user.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in DSharpPlus.Interactivity.InteractivityConfiguration.Timeout.</param>
        /// <returns>A tuple containing a <see cref="ConfirmationResult"/> and the associated <see cref="ComponentInteractionCreateEventArgs"/>.</returns>
        public static async Task<(ConfirmationResult, ComponentInteractionCreateEventArgs)> WaitForConfirmationInteraction(this CommandContext ctx, string content, TimeSpan? timeoutOverride = null)
        {
            var builder = new DiscordMessageBuilder()
                .WithContent(content)
                .AddComponents(Buttons);

            var message = await ctx.RespondAsync(builder);
            builder.Clear();

            var interactivityResult = await message.WaitForButtonAsync(user: ctx.User, timeoutOverride);
            if (interactivityResult.TimedOut)
            {
                builder.WithContent($"{content}\n\nRequest timed out.");
                await message.ModifyAsync(builder);
                return (ConfirmationResult.TimedOut, null);
            }
            else
            {
                var buttonPress = interactivityResult.Result;
                builder.WithContent(content);
                await message.ModifyAsync(builder);
                if (buttonPress.Id == DenyId)
                {

                    return (ConfirmationResult.Denied, buttonPress);
                }
                else
                {
                    return (ConfirmationResult.Confirmed, buttonPress);
                }
            }
        }

        /// <summary>
        /// Asks the user tied to the current context to confirm an action.
        /// </summary>
        /// <param name="ctx">The CommandContext to wait on.</param>
        /// <param name="embed">An embed containing the action or query to ask the user.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in DSharpPlus.Interactivity.InteractivityConfiguration.Timeout.</param>
        /// <returns>A tuple containing a <see cref="ConfirmationResult"/> and the associated <see cref="ComponentInteractionCreateEventArgs"/>.</returns>
        public static async Task<(ConfirmationResult, ComponentInteractionCreateEventArgs)> WaitForConfirmationInteraction(this CommandContext ctx, DiscordEmbed embed, TimeSpan? timeoutOverride = null)
        {
            var builder = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(Buttons);

            var message = await ctx.RespondAsync(builder);
            builder.Clear();
            builder.AddEmbed(embed);

            var interactivityResult = await message.WaitForButtonAsync(user: ctx.User, timeoutOverride);
            if (interactivityResult.TimedOut)
            {
                await message.ModifyAsync(builder);
                return (ConfirmationResult.TimedOut, null);
            }
            else
            {
                var buttonPress = interactivityResult.Result;
                await message.ModifyAsync(builder);
                if (buttonPress.Id == DenyId)
                {
                    return (ConfirmationResult.Denied, buttonPress);
                }
                else
                {
                    return (ConfirmationResult.Confirmed, buttonPress);
                }
            }
        }
    }
}
