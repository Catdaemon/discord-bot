﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using discord_bot.Services;

namespace discord_bot
{
    class Program
    {
        // setup our fields we assign later
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private bool shutdown = false;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            // create the configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            // build the configuration and assign to _config          
            _config = configBuilder.Build();

            // init db
            Database.InitDB();
        }

        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                // setup logging and the ready event
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                client.Disconnected += Disconnected;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot, _config["DISCORD_TOKEN"]);
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                while(!shutdown)
                {
                    await Task.Delay(1000);
                }
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task Disconnected(Exception e)
        {
            Console.WriteLine("(!) Disconnected: " + e.Message);
            shutdown = true;
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine("Connected.");
            return Task.CompletedTask;
        }

        // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
