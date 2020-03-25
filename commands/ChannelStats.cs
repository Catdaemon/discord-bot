using Discord;
using Dapper;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace discord_bot.Modules
{
    public class ChannelStats : ModuleBase
    {
        [Command("stats")]
        [Alias("channelstats")]
        public async Task GetChannelStats([Remainder]string args = null)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            embed.Title = "Channel Stats (last 30d)";

            List<string> messages;
            List<string> users;

            List<string> ignore = new List<string>()
            {
                "the",
                "and",
                "a",
                "an",
                "are",
                "at",
                "be",
                "by",
                "for",
                "from",
                "has",
                "he",
                "she",
                "in",
                "is",
                "it",
                "its",
                "of",
                "on",
                "that",
                "to",
                "was",
                "were",
                "will",
                "with",
                "out",
                "find",
                "i",
                "when"
            };

            var words = new Dictionary<string, int>();
            var userCounts = new Dictionary<string, int>();

            using (var connection = Database.GetConnection())
            {
                messages = connection.Query<string>("SELECT Message FROM ChatMessages WHERE Timestamp > DATETIME('now','-30 days')").AsList();
                users = connection.Query<string>("SELECT User FROM ChatMessages WHERE Timestamp > DATETIME('now','-30 days')").AsList();
            }

            foreach(var message in messages)
            {
                var _message = message.Replace(",", "").Replace(".", "").Replace("'", "").Replace("\"", "").ToLower();
                var _words = _message.Split(' ').Where(x => !ignore.Contains(x) && !x.StartsWith("!"));

                foreach(var _word in _words)
                {
                    if (words.ContainsKey(_word))
                    {
                        words[_word]++;
                    } else {
                        words.Add(_word, 1);
                    }
                }
            }

            foreach(var user in users)
            {
                if (userCounts.ContainsKey(user))
                {
                    userCounts[user]++;
                } else {
                    userCounts.Add(user, 1);
                }
            }
            
            var topten = words.OrderByDescending(x => x.Value).Take(10);
            int current = 1;

            foreach(var word in topten)
            {
                sb.AppendLine($"{current}: {word.Key} ({word.Value} times)");
                current++;
            }

            embed.AddField("Top 10 words used", sb.ToString());

            topten = userCounts.OrderByDescending(x => x.Value).Take(10);
            current = 1;
            sb = new StringBuilder();

            foreach(var user in topten)
            {
                sb.AppendLine($"{current}: {user.Key} ({user.Value} messages)");
                current++;
            }

            embed.AddField("Lords of the Spam", sb.ToString());

            await ReplyAsync(null, false, embed.Build());
        }
    }
}