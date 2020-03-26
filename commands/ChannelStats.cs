using Discord;
using Dapper;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace discord_bot.Modules
{
    public class ChannelStats : ModuleBase
    {
        private IConfiguration configuration;
        private List<string> ignoredWords = new List<string>();
        private string commandPrefix = "!";

        public ChannelStats(IServiceProvider services)
        {
            configuration = services.GetRequiredService<IConfiguration>();
            configuration.Bind("ExcludeWords", ignoredWords);
            commandPrefix = configuration["COMMAND_PREFIX"];
        }

        [Command("stats")]
        [Alias("channelstats")]
        public async Task GetChannelStats([Remainder]string args = null)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            embed.Title = "Channel Stats (last 30d)";

            List<string> messages;
            List<string> users;

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
                var _words = _message.Split(' ').Where(x => !ignoredWords.Contains(x) && !x.StartsWith("!"));

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