﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Humanizer;
using MutinyBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MutinyBot
{
    public class MutinyBotHelpFormatter : BaseHelpFormatter
    {
        public DiscordEmbedBuilder EmbedBuilder { get; }
        private Command Command { get; set; }
        public MutinyBot MutinyBot { protected get; set; }

        /// <summary>
        /// Creates a new default help formatter.
        /// </summary>
        /// <param name="ctx">Context in which this formatter is being invoked.</param>
        public MutinyBotHelpFormatter(CommandContext ctx, MutinyBot mutinyBot)
            : base(ctx)
        {
            this.EmbedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Help")
                .WithColor(0x007FFF);

            MutinyBot = mutinyBot;
        }

        /// <summary>
        /// Sets the command this help message will be for.
        /// </summary>
        /// <param name="command">Command for which the help message is being produced.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.Command = command;
            CommandOverload overload;
            var sBuilder = new StringBuilder();

            sBuilder.Append($"{Formatter.InlineCode(command.Name)}: {command.Description ?? "No description provided."}");

            if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
                sBuilder.Append($"\n\nThis group can be executed as a standalone command.");

            sBuilder.Append("\n\n");
            sBuilder.Append($"An argument wrapped by `<>` denotes a mandatory argument, while those in `[]` is optional." +
                $" As a reminder, my prefixes are any of the following: {String.Join(", ", MutinyBot.GetCommandPrefixes().Select(x => $"`{x}`"))}.");

            this.EmbedBuilder.WithDescription(sBuilder.ToString());

            if (command.Aliases?.Any() == true)
                this.EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);


            sBuilder.Clear();
            var commandOverloads = command.Overloads.OrderByDescending(x => x.Priority);
            for (int i = 1; i <= commandOverloads.Count(); i++)
            {
                overload = commandOverloads.ElementAt(i - 1);

                sBuilder.Append($"`{command.QualifiedName}");

                foreach (var arg in overload.Arguments)
                    sBuilder.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                sBuilder.Append("`\n");

                foreach (var arg in overload.Arguments)
                    sBuilder.Append('`').Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');

                this.EmbedBuilder.AddField($"{i.ToOrdinalWords().First().ToString().ToUpper() + i.ToOrdinalWords().Substring(1)} Overload", sBuilder.ToString().Trim(), false);
                sBuilder.Clear();
            }

            return this;
        }

        /// <summary>
        /// Sets the subcommands for this command, if applicable. This method will be called with filtered data.
        /// </summary>
        /// <param name="subcommands">Subcommands for this command group.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (this.Command != null) //help for a specific command
            {
                this.EmbedBuilder.AddField("Subcommands", string.Join(", ", subcommands.Select(x => Formatter.InlineCode(x.Name))), false);
            }
            else
            {
                var groupedCommands = new List<CommandGroup>();
                var unGroupCommands = new List<string>();
                foreach (var command in subcommands)
                {
                    switch (command)
                    {
                        case CommandGroup group:
                            groupedCommands.Add(group);
                            break;
                        default:
                            unGroupCommands.Add(command.Name);
                            break;
                    }
                }
                foreach (var group in groupedCommands.OrderBy(x => x.Children.Count))
                {
                    this.EmbedBuilder.AddField(group.QualifiedName.CapitalizeFirst(), (group.IsExecutableWithoutSubcommands ? $"`{group.Name}`, " : null) + string.Join(", ", group.Children.Select(x => Formatter.InlineCode(x.Name))), false);
                }
                this.EmbedBuilder.AddField("Other Commands", string.Join(", ", unGroupCommands.Select(x => Formatter.InlineCode(x))), false);
            }
            return this;
        }

        /// <summary>
        /// Construct the help message.
        /// </summary>
        /// <returns>Data for the help message.</returns>
        public override CommandHelpMessage Build()
        {
            if (this.Command == null)
                this.EmbedBuilder.WithDescription("Listing all top-level commands and groups. Specify a command to see more information.");

            return new CommandHelpMessage(embed: this.EmbedBuilder.Build());
        }
    }
}
