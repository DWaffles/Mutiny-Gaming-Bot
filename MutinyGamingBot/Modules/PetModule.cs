using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using MutinyBot.Common;
using MutinyBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [RequireGuild()]
    [Group("pet"), Aliases("pets")]
    [Description("Gets a random pet for this server.")]
    [Cooldown(4, 15, CooldownBucketType.Channel)]
    public class PetModule : MutinyBotModule
    {
        // Allowed file types within embeds: JPG PNG GIF WEBP
        // Not allowed: .mp4 .webm
        private readonly Regex fileTypePattern = new(".(png|jp(e)?g|gif|webp)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        [GroupCommand]
        public async Task GetPet(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            PetEntity pet = PetService.GetRandomGuildPet(ctx.Guild.Id);
            if (pet is null)
            {
                await ctx.RespondAsync($"There are no pets for this server.");
                return;
            }
            DiscordMember owner = await ctx.Guild.GetMemberAsync(pet.OwnerId);

            await ctx.RespondAsync(embed: GetPetEmbed(author: ctx.Member, owner: owner, pet: pet));
        }
        [Command("from")]
        [Description("Gets a random pet from a member.")]
        public async Task GetPetByOwner(CommandContext ctx,
            [Description("Pinged user to get pet from")] DiscordMember owner)
        {
            await ctx.TriggerTypingAsync();

            PetEntity pet = PetService.GetRandomPetFromMember(owner);
            if (pet is null)
            {
                await ctx.RespondAsync($"{owner.DisplayName} has not added any pets for this server.");
                return;
            }
            await ctx.RespondAsync(embed: GetPetEmbed(author: ctx.Member, owner: owner, pet: pet));
        }
        [Command("from")]
        public async Task GetPetByOwner(CommandContext ctx,
            [Description("Display or username of member to get pet from")][RemainingText] string owner)
        {
            await ctx.TriggerTypingAsync();

            var members = await ctx.Guild.GetAllMembersAsync();
            var foundMember = GetMemberByName(members, owner);

            if (foundMember is not null)
                await GetPetByOwner(ctx, foundMember);
            else
                await ctx.RespondAsync(GetMemberNotFoundEmbed());
        }
        [Command("id")]
        [Description("List all pets in this server, or list all pets from a specific member if given.")]
        public async Task GetPetById(CommandContext ctx, int uniquePetId)
        {
            await ctx.TriggerTypingAsync();

            PetEntity pet = PetService.GetPetFromId(uniquePetId);
            if (pet is null)
            {
                await ctx.RespondAsync($"There no pets with the id of {uniquePetId}.");
                return;
            }
            if(pet.GuildId != ctx.Guild.Id)
            {
                await ctx.RespondAsync($"This given pet is not for {ctx.Guild.Name}.");
                return;
            }
            DiscordMember owner = await ctx.Guild.GetMemberAsync(pet.OwnerId);

            await ctx.RespondAsync(embed: GetPetEmbed(author: ctx.Member, owner: owner, pet: pet));
        }
        [Command("list")]
        [Description("List all pets in this server, or list all pets from a specific member if given.")]
        public async Task ListAllPets(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var pets = PetService.GetAllGuildPets(ctx.Guild.Id);

            if (!pets.Any())
            {
                await ctx.RespondAsync($"There no pets for {ctx.Guild.Name}.");
                return;
            }
            pets = pets.OrderBy(x => x.Id);
            int maxCount = 25;

            if (pets.Count() <= maxCount)
            {
                await ctx.RespondAsync(GetPetListEmbed(ctx.Guild, pets));
            }
            else //paginated embed
            {
                List<Page> pages = GetInteractivePetListPages(ctx.Guild, pets, maxCount);
                var interactivity = ctx.Client.GetInteractivity();
                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages); // override to 2 minutes instead?
            }
        }
        [Command("list")]
        public async Task ListAllPetsFromUser(CommandContext ctx, 
            [Description("[OPTIONAL] Ping the member to search pets for.")] DiscordMember owner = null)
        {
            await ctx.TriggerTypingAsync();

            var pets = PetService.GetAllPetsFromMember(owner);

            if (pets.Any())
            {
                await ctx.RespondAsync($"There no pets for {ctx.Guild.Name}.");
                return;
            }
        }
        [Command("list")]
        public async Task ListAllPetsFromUser(CommandContext ctx, 
            [Description("[OPTIONAL] Display or username of the member to search pets for."), RemainingText] string memberName)
        {
            var foundMember = await GetMemberByNameAsync(ctx.Guild, memberName);
            if(foundMember is null)
            {
                await ctx.RespondAsync(embed: GetMemberNotFoundEmbed());
                return;
            }
            await ListAllPetsFromUser(ctx, foundMember);
        }
        [Command("add")]
        [Description("To add a pet, attach an image or give a link, and *then* give the name of the pet. File types must be .PNG, .JPEG, .GIF, or .WEBP")]
        public async Task AddPet(CommandContext ctx,
            [Description("Given image link.")] Uri imageUrl,
            [Description("Can be multiple words."), RemainingText] string petName)
        {
            await AddPet(ctx, petName);
        }
        [Command("add")]
        public async Task AddPet(CommandContext ctx,
            [Description("Can be multiple words."), RemainingText] string petName)
        {
            await ctx.TriggerTypingAsync();
            var memberEntity = await MemberService.GetOrCreateMemberAsync(ctx.Member);
            if (!memberEntity.VerifiedPetOwner)
            {
                await ctx.RespondAsync("You are not allowed to add pets to this server.");
                return;
            }

            string imageUrl = UtilityHelpers.FirstImageOrVideoUrl(ctx.Message);
            if (string.IsNullOrEmpty(imageUrl))
            {
                await ctx.RespondAsync("Please send a video or image with the command.");
                return;
            }
            else if (!fileTypePattern.Match(imageUrl).Success)
            {
                await ctx.RespondAsync("The attachment has a file type that I cannot embed yet because of Discord API limitations.");
                return;
            }

            PetEntity newPet = new()
            {
                PetName = petName,
                OwnerId = ctx.Member.Id,
                GuildId = ctx.Guild.Id,
                PetImageUrl = imageUrl
            };
            await PetService.AddPetAsync(newPet);
            await ctx.RespondAsync($"Added {petName}");
        }
        [Command("remove")]
        [Description("Give a pet's unique pet ID to remove it from the pet list. You must be the pet's owner.")]
        public async Task RemovePet(CommandContext ctx, [Description("The unique pet ID.")] int uniquePetId)
        {
            await ctx.TriggerTypingAsync();

            PetEntity pet = PetService.GetPetFromId(uniquePetId);

            if (pet is null)
            {
                await ctx.RespondAsync("There is no pet by that ID.");
                return;
            }
            if (pet.OwnerId == ctx.Member.Id)
            {
                await PetService.RemovePetAsync(pet);
                await ctx.RespondAsync(GetRemovedPetEmbed(ctx.Member, pet));
            }
            else
            {
                await ctx.RespondAsync("You do not have any pets to remove.");
            }
        }
        private DiscordEmbed GetPetEmbed(DiscordMember author, DiscordMember owner, PetEntity pet)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor($"{author.DisplayName} requested a pet", iconUrl: author.AvatarUrl)
                .WithDescription(pet.PetName is not null ? $"{Formatter.Bold(pet.PetName)} from {owner.Mention}"
                : $"Pet from {owner.Mention}")
                .WithImageUrl(pet.PetImageUrl)
                .WithFooter($"1 out of {PetService.GetNumberPetsFromMember(owner)} pets from {owner.DisplayName}. Unique ID: {pet.Id}")
                .WithTimestamp(DateTime.Now)
                .WithColor(new DiscordColor(MutinyBot.Config.HexCode));
        }
        private DiscordEmbed GetRemovedPetEmbed(DiscordMember owner, PetEntity pet)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor($"{owner.DisplayName} removed a pet", iconUrl: owner.AvatarUrl)
                .WithDescription(pet.PetName is not null ? $"{Formatter.Bold(pet.PetName)}"
                : $"No name given by {owner.Mention}")
                .WithImageUrl(pet.PetImageUrl)
                .WithFooter($"Unique ID: {pet.Id}")
                .WithTimestamp(DateTime.Now)
                .WithColor(new DiscordColor(MutinyBot.Config.HexCode));
        }
        private DiscordEmbed GetPetListEmbed(DiscordGuild guild, IEnumerable<PetEntity> pets)
        {
            List<string> petNames = new();
            List<string> ownerNames = new();
            List<int> petIds = new();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"All Pets for {guild.Name}", iconUrl: guild.IconUrl)
                .WithColor(new DiscordColor(MutinyBot.Config.HexCode));

            foreach (PetEntity pet in pets)
            {
                petNames.Add(pet.PetName ?? "No Name Given");
                ownerNames.Add($"<@{pet.OwnerId}>");
                petIds.Add(pet.Id);
            }
            embed.AddField($"Pet", string.Join("\n", petNames), true);
            embed.AddField($"Owner", string.Join("\n", ownerNames), true);
            embed.AddField($"ID", string.Join("\n", petIds), true);
            return embed;
        }
        private static List<Page> GetInteractivePetListPages(DiscordGuild guild, IEnumerable<PetEntity> pets, int maxCount)
        {
            List<Page> pages = new();
            int numPages = (int)Math.Ceiling((double)pets.Count() / maxCount);

            for (int pageIndex = 0; pageIndex < numPages; pageIndex++)
            {

            }

            return pages;
        }
    }
}
