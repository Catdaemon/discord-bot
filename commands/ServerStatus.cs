using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Net;
using SourceServerQuery;

namespace discord_bot.Modules
{
    public class ServerStatus : ModuleBase
    {
        public class Server {
            public string Name {get; set;}
            public string Address {get; set;}
            public int Port {get; set;}
        }

        private IConfiguration configuration;
        private List<Server> servers = new List<Server>();

        public ServerStatus(IServiceProvider services)
        {
            configuration = services.GetRequiredService<IConfiguration>();
            configuration.Bind("Servers", servers);
        }

        [Command("servers")]
        public async Task GetStatus([Remainder]string args = null)
        {            
            foreach(var server in servers)
            {
                var embed = new EmbedBuilder();
                embed.Title = server.Name;
                embed.WithColor(new Color(0, 255,0));

                try {
                    var ip = Dns.GetHostAddresses(server.Address).FirstOrDefault();
                    var endPoint = new IPEndPoint(ip, server.Port);
                    var q = new A2S_INFO(endPoint);
                    var players = new A2S_PLAYER(endPoint);
                    var playerString = new StringBuilder();

                    foreach(var player in players.Players)
                    {
                        playerString.AppendLine(player.Name);
                    }

                    if (players.Players.Count() == 0)
                    {
                        playerString.AppendLine("There are no players.");
                    }

                    embed.AddField("Map", q.Map);
                    embed.AddField("Players", $"{q.Players}/{q.MaxPlayers}");
                    embed.AddField("Player List", playerString.ToString());
                    
                } catch (Exception e) {
                    embed.WithColor(new Color(255, 0,0));
                    embed.Description = "Server is not responding.";
                }

                _ = ReplyAsync(null, false, embed.Build());
            }
        }
    }
}