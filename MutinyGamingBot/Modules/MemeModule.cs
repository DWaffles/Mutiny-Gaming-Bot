using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    [Hidden]
    public class MemeModule : MutinyBotModule
    {
        [Command("tea"), Aliases("chart")]
        [Description("Returns the Anglo's favorite chart.")]
        public async Task TeaChartCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"https://i.imgur.com/6v0VFI4.png");
        }
        [Command("bowlingball"), Aliases("bowling", "ball")]
        [Description("It's a joke @Gooby, god @Gooby you just use the same humor all the time dont you ? youre not funny, you just put in random bowling ball puns everywhere which isnt funny. Just stop you use the same joke everyone else uses gosh @Gooby you couldnt even strike me if you wanted to because my humor is above your brain level. My humor is for intellects only.")]
        public async Task BowlingBallCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var rand = Rand.Next(0, 7);
            switch (rand)
            {
                case 0:
                    await ctx.RespondAsync("*ahem* Gentlemen.\n> To be fair, you have to have a very high IQ to understand being bowling balled. The humour is extremely subtle, and without a solid grasp of theoretical physics most of the jokes will go over a typical Mutineer's head. There's also Gooby's superior humour, which is deftly woven into his characterisation- his personal philosophy draws heavily from constant baiting, for instance. The fans understand this stuff; they have the intellectual capacity to truly appreciate the depths of these jokes, to realise that they're not just funny- they say something deep about LIFE. As a consequence people who dislike being bowling balled truly ARE idiots- of course they wouldn't appreciate, for instance, the humour in Gooby's existential catchphrase \"you just got bowling balled,\" which itself is a cryptic reference to haha get bowling balled. I'm smirking right now just imagining one of those addlepated simpletons scratching their heads in confusion as Gooby's genius wit unfolds itself on their television screens. What fools.. how I pity them.");
                    break;
                case 1:
                    await ctx.RespondAsync("https://i.imgur.com/5jUVJpK.png");
                    break;
                case 2:
                    await ctx.RespondAsync("||https://i.imgur.com/5TaP6hz.png||");
                    break;
                case 3:
                    await ctx.RespondAsync("https://youtu.be/El_hlnHnxDw");
                    break;
                case 4:
                    await ctx.RespondAsync("It's a joke @Gooby, god @Gooby you just use the same humor all the time dont you ? youre not funny, you just put in random bowling ball puns everywhere which isnt funny. Just stop you use the same joke everyone else uses gosh @Gooby you couldnt even strike me if you wanted to because my humor is above your brain level. My humor is for intellects only");
                    break;
                case 5:
                    await ctx.RespondAsync("https://i.imgur.com/DHGmUeL.jpeg");
                    break;
                case 6:
                    await ctx.RespondAsync("https://i.imgur.com/5TCYEzr.jpeg");
                    break;
                default:
                    await ctx.RespondAsync("https://i.imgur.com/5TCYEzr.jpeg");
                    break;
            }
        }
        [Command("asshole"), Aliases("veterans", "ball")]
        [Description("\"The official chart of assholeness of all the active veterans\" - @Luke")]
        public async Task VeteranChartCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var rand = Rand.Next(0, 2);
            switch (rand)
            {
                case 0:
                    await ctx.RespondAsync("https://i.imgur.com/iJUiOHK.png");
                    break;
                case 1:
                    await ctx.RespondAsync("https://i.imgur.com/czPnTPe.png");
                    break;
            }
        }
        // Its been 0 days since command channel
    }
}
