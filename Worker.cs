using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetDiscordBotAcs
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting discord bot");
                await Task.Delay(1000, stoppingToken);
                using var discord = new DiscordClient(new DiscordConfiguration()
                {
                    Token = _configuration["DiscordBotToken"],
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged     
                });

                discord.MessageCreated += async (s, e) =>
                {
                    if (e.Message.Content.ToLower().StartsWith("ping")) 
                    {
                        _logger.LogInformation("pinged, responding with pong!");
                        await e.Message.RespondAsync("pong!");
                    }
                };

                await discord.ConnectAsync();
                await Task.Delay(-1);
                
                _logger.LogInformation("Discord bot stopped");
            }

            _logger.LogInformation("Cancellation requested");
        }
    }
}
