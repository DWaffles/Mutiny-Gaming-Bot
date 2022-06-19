using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using MutinyBot.Models;
using MutinyBot.Services;
using Serilog;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MutinyBot.Modules.SlashModules
{
    [SlashCommandGroup("pet", "Commands relating to Mutiny's pets!")]
    [SlashRequireGuild]
    public class PetSlashModule : SlashModule
    {
        // Embed, allowed file types: JPG PNG GIF WEBP
        // Embed, not allowed types: .mp4 .webm
        // Imgur, allowed types: JPEG PNG GIF APNG
        // Imgur, not allowed types: .webp GIFV
        private Regex FileTypePattern { get; } = new(".(png|jp(e)?g|gif)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        [SlashCommand("random", "Gets a random pet from this guild.")]
        public async Task GetPetCommand(InteractionContext ctx)
        {
            PetImageModel pet = PetService.GetPetFromGuild(ctx.Guild.Id);
            if (pet is null)
                await ctx.CreateResponseAsync($"There are no pets for this server.");
            else
            {
                var owner = await ctx.Client.GetUserAsync(pet.OwnerId);
                await ctx.CreateResponseAsync(embed: GetPetEmbed(pet, ctx.User as DiscordMember, owner));
            }
        }
        [SlashCommand("from", "Gets a pet from this guild by the pet id.")]
        public async Task GetPetFromCommand(InteractionContext ctx, [Option("member", "member to get a pet from")] DiscordUser owner)
        {
            PetImageModel pet = PetService.GetPetByMember(ctx.Guild, owner);
            if (pet is null)
                await ctx.CreateResponseAsync($"There are no pets from this member.");
            else
            {
                await ctx.CreateResponseAsync(embed: GetPetEmbed(pet, ctx.User as DiscordMember, owner));
            }
        }
        [SlashCommand("id", "Gets a pet from this guild by the pet id.")]
        public async Task GetPetFromCommand(InteractionContext ctx, [Option("id", "pet id to search for")] long id)
        {
            PetImageModel pet = PetService.GetPetById(Convert.ToInt32(id));
            if (pet is null)
                await ctx.CreateResponseAsync($"There are no pets of this id in this guild.");
            else
            {
                var owner = await ctx.Client.GetUserAsync(pet.OwnerId);
                await ctx.CreateResponseAsync(embed: GetPetEmbed(pet, ctx.User as DiscordMember, owner));
            }
        }
        [SlashCommand("add", "Adds a pet.")]
        [IsPetOwnerSlash]
        public async Task AddPetCommand(InteractionContext ctx,
            [Option("image", "Image of the pet. Accepted file types: PNG, JPEG, GIF. Max size 20MB.")] DiscordAttachment image,
            [Option("names", "The names of the pets in the image.")] string names)
        {
            if (!FileTypePattern.IsMatch(image.FileName))
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("The attachment has a file type that I cannot embed yet because of Discord API limitations. File types I can embed are .PNG, .JPEG, .GIF, and .WEBP").AsEphemeral());
                return;
            }
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var (response, imgurUrl, deleteHash) = await ImgurService.UploadImage(image.Url); //TEST WEBP

            if(response.IsSuccessful)
            {
                PetImageModel pet = new()
                {
                    OwnerId = ctx.User.Id,
                    GuildId = ctx.Guild.Id,
                    PetNames = names,
                    MediaUrl = imgurUrl,
                    ImgurDeleteHash = deleteHash,
                };

                await PetService.AddPetAsync(pet);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(AddPetEmbed(pet, ctx.User as DiscordMember)));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Uploading the given pet image to Imgur failed."));
                Log.Logger.Error($"Error occured while attempting to upload an image to Imgur. Message: {response.ErrorMessage}");
            }
        }

        [SlashCommand("remove", "Adds a pet.")]
        public async Task DeletePetCommand(InteractionContext ctx, [Option("id", "Unique pet id of the image to delete")] long petId)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            PetImageModel pet = PetService.GetPetById(Convert.ToInt32(petId));
            if (pet == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("There is no pet with this id."));
            }
            else if (pet.OwnerId == ctx.User.Id)
            {
                bool? imgurDelete = null;

                if (!String.IsNullOrEmpty(pet.ImgurDeleteHash))
                {
                    (_, imgurDelete) = await ImgurService.DeleteImage(pet.ImgurDeleteHash);
                }
                await PetService.RemovePetAsync(pet);
                if (imgurDelete != null)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(""));
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(""));
                }
            }
            else
            {

            }
        }

        /*[SlashCommand("edit", "Edits a pet.")]
        public async Task EditPetCommand(InteractionContext ctx, [Option("id", "member to get a pet from")] long petId)
        {

        }*/

        #region EmbedMethods
        private DiscordEmbed GetPetEmbed(PetImageModel pet, DiscordMember requester, DiscordUser owner)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor($"{requester.DisplayName} requested a pet", iconUrl: requester.AvatarUrl)
                .WithDescription($"{Formatter.Bold(pet.PetNames)} from {owner.Mention}.")
                .WithImageUrl(pet.MediaUrl)
                .WithFooter($"1 out of {PetService.CountPets(requester.Guild.Id, owner.Id)} pets from {owner.Username} • Unique ID: {pet.ImageId}")
                .WithTimestamp(DateTime.Now)
                .WithColor(GetBotColor());
        }
        private DiscordEmbed AddPetEmbed(PetImageModel pet, DiscordMember owner)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor($"{owner.DisplayName} added pets to {owner.Guild.Name}", iconUrl: owner.AvatarUrl)
                .WithDescription($"{owner.Mention} added {Formatter.Bold(pet.PetNames)}.")
                .WithImageUrl(pet.MediaUrl)
                .WithFooter($"1 out of {PetService.CountPets(owner.Guild.Id, owner.Id)} pets from {owner.Username} • Unique ID: {pet.ImageId}")
                .WithTimestamp(DateTime.Now)
                .WithColor(GetBotColor());
        }
        #endregion
    }
}
