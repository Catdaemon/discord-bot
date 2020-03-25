using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace discord_bot.Services
{
    public class CommandHandler
    {
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            
            _commands.CommandExecuted += CommandExecutedAsync;

            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) 
            {
                return;
            }
            
            if (message.Source != MessageSource.User) 
            {
                return;
            }

            // Log message
            _ = Database.LogAsync(message.Channel.Id.ToString(), message.Author.Username, message.Content);
            _ = Database.LogChannelAsync(message.Channel.Id.ToString(), message.Channel.Name);

            var argPos = 0;

            char prefix = Char.Parse(_config["COMMAND_PREFIX"]);

            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos))) 
            {
                return;
            }
           
            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _services); 
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                return;
            }
                
            if (result.IsSuccess)
            {
                return;
            }
                
            await context.Channel.SendMessageAsync(result.Error.Value.ToString());
        }        
    }
}