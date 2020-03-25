using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace discord_bot.Modules
{
    public class EightBalls : ModuleBase
    {
        [Command("8ball")]
        [Alias("ask")]
        public async Task AskEightBall([Remainder]string args = null)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            var replies = new List<string>();

            replies.Add("yh fam");
            replies.Add("yes.. but also no");
            replies.Add("nah");
            replies.Add("it's a ting");
            replies.Add("it's not a ting");
            replies.Add("bitch it might be");

            embed.WithColor(new Color(0, 255,0));
            embed.Title = "Eight Balls:";
            
            if (args == null)
            {
                sb.AppendLine("how you gonna ask man nothing for");
            }
            else 
            {
                var answer = replies[new Random().Next(replies.Count - 1)];
                
                embed.AddField("Question", args);
                embed.AddField("Answer", answer);
            }

            await ReplyAsync(null, false, embed.Build());
        }
    }
}