namespace MutinyBot.Models
{
    /// <summary>
    /// Represents a Discord user within the database.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets the Id of the user.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// Gets whether this user is banned from interacting with the bot.
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        /// Gets whether this can add pets to the bot.
        /// </summary>
        /// <remarks>This is more of a symbol of trust allowing users to add content to the bot.</remarks>
        public bool IsPetOwner { get; set; }

        /// <summary>
        /// Constructs a new instance of this model.
        /// </summary>
        public UserModel()
        {
            IsBanned = false;
            IsPetOwner = false;
        }
    }
}
