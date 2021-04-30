using DSharpPlus.Entities;
using MutinyBot.Database;
using MutinyBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutinyBot.Services
{
    public interface IPetService
    {
        PetEntity GetRandomGuildPet(ulong guildId);
        PetEntity GetRandomPetFromMember(DiscordMember member);
        PetEntity GetPetFromId(int UniquePetId);
        IEnumerable<PetEntity> GetAllGuildPets(ulong guildId);
        IEnumerable<PetEntity> GetAllPetsFromMember(DiscordMember member);
        int GetNumberPetsFromMember(DiscordMember member);
        Task AddPetAsync(PetEntity pet);
        Task RemovePetAsync(PetEntity pet);

    }
    public class PetService : IPetService
    {
        private readonly MutinyBotDbContext dbContext;
        private readonly Random rand;
        public PetService(MutinyBotDbContext dbContext, Random rand)
        {
            Console.WriteLine("PETSERVICE");
            this.dbContext = dbContext;
            this.rand = rand;
        }
        public PetEntity GetRandomGuildPet(ulong guildId)
        {
            if (dbContext.Pets.Any(x => x.GuildId == guildId))
            {
                List<PetEntity> pets = dbContext.Pets.Where(x => x.GuildId == guildId).ToList();
                return pets[rand.Next(pets.Count)];
            }
            else
            {
                return null;
            }
        }
        public PetEntity GetRandomPetFromMember(DiscordMember member)
        {
            if (dbContext.Pets.Any(x => x.GuildId == member.Guild.Id && x.OwnerId == member.Id))
            {
                List<PetEntity> pets = dbContext.Pets.Where(x => x.GuildId == member.Guild.Id && x.OwnerId == member.Id).ToList();
                return pets[rand.Next(pets.Count)];
            }
            else
            {
                return null;
            }
        }
        public PetEntity GetPetFromId(int UniquePetId)
        {
            return dbContext.Pets.SingleOrDefault(x => x.Id == UniquePetId);
        }
        public IEnumerable<PetEntity> GetAllGuildPets(ulong guildId)
        {
            return dbContext.Pets.Where(x => x.GuildId == guildId);
        }
        public IEnumerable<PetEntity> GetAllPetsFromMember(DiscordMember member)
        {
            return dbContext.Pets.Where(x => x.GuildId == member.Guild.Id && x.OwnerId == member.Id);
        }
        public int GetNumberPetsFromMember(DiscordMember member)
        {
            return dbContext.Pets.Count(x => x.GuildId == member.Guild.Id && x.OwnerId == member.Id);
        }
        public Task AddPetAsync(PetEntity pet)
        {
            dbContext.Pets.Add(pet);
            return dbContext.SaveChangesAsync();
        }
        public Task RemovePetAsync(PetEntity pet)
        {
            dbContext.Pets.Remove(pet);
            return dbContext.SaveChangesAsync();
        }
    }
}
