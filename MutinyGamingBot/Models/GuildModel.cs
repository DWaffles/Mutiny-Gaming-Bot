using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MutinyBot.Models
{
    /// <summary>
    /// Represents a Discord guild within the database.
    /// </summary>
    public class GuildModel
    {
        /// <summary>
        /// Gets the Id of this guild model.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Gets the Id of the moderation channel set within the guild, if any.
        /// </summary>
        public ulong ModerationLogChannelId { get; set; }

        /// <summary>
        /// Gets the Id of the join log channel set within the guild, if any.
        /// </summary>
        public ulong JoinLogChannelId { get; set; }

        /// <summary>
        /// Gets whether tracking and storing member's previous roles is enabled.
        /// </summary>
        public bool TrackMemberRoles { get; set; }

        /// <summary>
        /// Gets whether tracking and storing a member's last message timestamp is enabled.
        /// </summary>
        public bool TrackMessageTimestamps { get; set; }

        /// <summary>
        /// Gets a hashset of the members that have database representations that belong to this guild.
        /// </summary>
        public HashSet<MemberModel> Members { get; set; }

        /// <summary>
        /// Constructs a new instance of this.
        /// </summary>
        public GuildModel() => Members = new();

        /// <summary>
        /// Constructs a new instance of this class from a DiscordGuild object.
        /// </summary>
        /// <param name="guild"></param>
        public GuildModel(DiscordGuild guild)
        {
            GuildId = guild.Id;
            Members = guild.Members.Values.Select(x => new MemberModel(x, TrackMemberRoles)).ToHashSet();
        }

        /// <summary>
        /// Updates this current instance with the data from the DiscordGuild and list of DiscordMembers.
        /// </summary>
        /// <param name="discordGuild">The DiscordGuild that relates to this guild model.</param>
        /// <param name="discordMembers">A list DiscordMember objects that belong to this guild.</param>
        public void UpdateGuild(DiscordGuild discordGuild, IEnumerable<DiscordMember> discordMembers)
        {
            if (discordGuild.Id != this.GuildId)
                throw new ArgumentException("The passed DiscordGuild object does not relate to this GuildModel.");

            if (ModerationLogChannelId != 0 && !discordGuild.Channels.ContainsKey(ModerationLogChannelId))
                ModerationLogChannelId = 0;
            if (JoinLogChannelId != 0 && !discordGuild.Channels.ContainsKey(JoinLogChannelId))
                JoinLogChannelId = 0;

            var newMembers = discordMembers.ToDictionary(x => x.Id, x => x);
            foreach (var eMember in Members)
            {
                var dMember = discordMembers.SingleOrDefault(m => m.Id == eMember.MemberId);
                if (dMember != null) //member is a current member
                {
                    eMember.UpdateMember(dMember, TrackMemberRoles);
                    newMembers.Remove(dMember.Id);
                }
                else
                {
                    eMember.CurrentMember = false;
                }
            }
            Members.UnionWith(newMembers.Values.Select(x => new MemberModel(x, TrackMemberRoles)));
        }
    }
}