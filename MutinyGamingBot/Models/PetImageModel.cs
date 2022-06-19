using System.Collections.Generic;

namespace MutinyBot.Models
{
    /// <summary>
    /// Represents a pet image within the database.
    /// </summary>
    /// <remarks>Pet images are specific to a guild.</remarks>
    public class PetImageModel
    {
        /// <summary>
        /// Gets the database id for this pet image.
        /// </summary>
        public int ImageId { get; set; }

        /// <summary>
        /// Gets the Discord id of the pet images' creator.
        /// </summary>
        public ulong OwnerId { get; set; }

        /// <summary>
        /// Gets the Discord guild id this pet image is specific.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Gets the names of the pet(s) in this pet image.
        /// </summary>
        public string PetNames { get; set; }

        /// <summary>
        /// Gets the media image url for this pet image.
        /// </summary>
        public string MediaUrl { get; set; }

        /// <summary>
        /// Gets the delete hash to delete this pet image off Imgur's servers.
        /// </summary>
        public string ImgurDeleteHash { get; set; }

        /// <summary>
        /// Constructs a new instance of this model.
        /// </summary>
        public PetImageModel() { }
    }
}