using DSharpPlus.Entities;
using MutinyBot.Database;
using MutinyBot.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    /// <summary>
    /// The service responsible for interacting with the database in relation to <see cref="PetImageModel"/>s.
    /// </summary>
    public class PetService
    {
        private Random Random { get; }

        public PetService(Random random)
            => Random = random;

        /// <summary>
        /// Gets a random pet image for the guild, or null if there is none.
        /// </summary>
        /// <param name="guildId">The id of the guild to get the pet image from.</param>
        /// <returns>A random <see cref="PetImageModel"/> from the guild with the given id.</returns>
        public PetImageModel GetPetFromGuild(ulong guildId)
        {
            using var context = new MutinyBotDbContext();
            if (context.Pets.Any(x => x.GuildId == guildId))
            {
                var pets = context.Pets.Where(x => x.GuildId == guildId).ToList();
                return pets[Random.Next(pets.Count)];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a random pet image for the guild that is from the given owner, or null if there is none that fit.
        /// </summary>
        /// <param name="guild">The guild object the pet image should belong to.</param>
        /// <param name="owner">The user object the pet image should belong to.</param>
        /// <returns>A random <see cref="PetImageModel"/> from the given guild with the given owner.</returns>
        public PetImageModel GetPetByMember(DiscordGuild guild, DiscordUser owner)
        {
            using var context = new MutinyBotDbContext();
            if (context.Pets.Any(x => x.GuildId == guild.Id && x.OwnerId == owner.Id))
            {
                var pets = context.Pets.Where(x => x.GuildId == guild.Id && x.OwnerId == owner.Id).ToList();
                return pets[Random.Next(pets.Count)];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a pet image with the given id, or null if there is none that fit.
        /// </summary>
        /// <param name="petId">The unique pet image id to look for.</param>
        /// <returns>The <see cref="PetImageModel"/> with the given model, or null if it was not found.</returns>
        public PetImageModel GetPetById(int petId)
        {
            using var context = new MutinyBotDbContext();
            return context.Pets.SingleOrDefault(x => x.ImageId == petId);
        }

        /// <summary>
        /// Gets the numerical count of pet images a specific owner has within a guild.
        /// </summary>
        /// <param name="guildId">The guild id the pet images belong to.</param>
        /// <param name="ownerId">The owner id of the pet images.</param>
        /// <returns></returns>
        public int CountPets(ulong guildId, ulong ownerId)
        {
            using var context = new MutinyBotDbContext();
            return context.Pets.Count(x => x.GuildId == guildId && x.OwnerId == ownerId);
        }

        public Task AddPetAsync(PetImageModel pet)
        {
            using var context = new MutinyBotDbContext();
            context.Pets.Add(pet);
            return context.SaveChangesAsync();
        }

        public Task RemovePetAsync(PetImageModel pet)
        {
            using var context = new MutinyBotDbContext();
            context.Pets.Remove(pet);
            return context.SaveChangesAsync();
        }
    }
}
