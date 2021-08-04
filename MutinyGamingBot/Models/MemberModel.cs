using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MutinyBot.Models
{
    /// <summary>
    /// Represents a Discord member within the database.
    /// </summary>
    public class MemberModel
    {
        /// <summary>
        /// Gets the Id of this member model.
        /// </summary>
        public ulong MemberId { get; set; }

        /// <summary>
        /// Gets the Id of the guild this member belongs to.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Gets whether or not this member is a current member of the related guild.
        /// </summary>
        public bool CurrentMember { get; set; }

        /// <summary>
        /// Gets how many times the bot has logged this member joining this guild.
        /// </summary>
        public int TimesJoined { get; set; }

        /// <summary>
        /// Gets how many times this user was officially muted using the bot.
        /// </summary>
        public int TimesMuted { get; set; }

        /// <summary>
        /// Gets the stored timestamp of the member's last message in DateTimeOffset.
        /// </summary>
        public DateTimeOffset LastMessageTimestamp
        {
            get => DateTimeOffset.FromUnixTimeSeconds(LastMessageTimestampRaw); set => LastMessageTimestampRaw = value.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Gets the stored timestamp of the member's last message in a Unix timestamp.
        /// </summary>
        public long LastMessageTimestampRaw { get; private set; }

        /// <summary>
        /// Gets a dictionary of roles that this member has had and currently has.
        /// </summary>
        public Dictionary<ulong, bool> RoleDictionary { get; set; }

        /// <summary>
        /// Gets the guild model this member belongs to.
        /// </summary>
        public GuildModel Guild { get; set; }

        /// <summary>
        /// Constructs a new instance of this model.
        /// </summary>
        public MemberModel()
        {
            CurrentMember = true;
            TimesJoined = 1;
            RoleDictionary = new();
        }

        /// <summary>
        /// Constructs a new instance of this model from the data in the parameters.
        /// </summary>
        /// <param name="member">The DiscordMember object that relates to this model.</param>
        /// <param name="trackRoles">Whether to include role information or not.</param>
        public MemberModel(DiscordMember member, bool trackRoles)
        {
            MemberId = member.Id;
            GuildId = member.Guild.Id;

            CurrentMember = true;
            TimesJoined = 1;

            RoleDictionary = trackRoles ? member.Roles.ToDictionary(role => role.Id, value => true) : new();
        }

        /// <summary>
        /// Updates this model from the data in the DiscordMember object.
        /// </summary>
        /// <param name="member">The DiscordMember object that relates to this model.</param>
        /// <param name="trackRoles">Whether to record role changes since the last time this model was updated.</param>
        public void UpdateMember(DiscordMember member, bool trackRoles)
        {
            if (!CurrentMember)
                TimesJoined++;

            CurrentMember = true;

            if (trackRoles)
            {
                var memberRoles = member.Roles.Select(x => x.Id);
                var cachedRoles = RoleDictionary.Where(pair => pair.Value == true).Select(pair => pair.Key);

                foreach (var roleId in memberRoles.Except(cachedRoles)) //only in memberRoles
                {
                    RoleDictionary[roleId] = true;
                }
                foreach (var roleId in cachedRoles.Except(memberRoles)) //only in cachedRoles
                {
                    RoleDictionary[roleId] = false;
                }
            }
        }
    }
}
