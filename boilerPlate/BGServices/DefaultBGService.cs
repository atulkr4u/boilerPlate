using System;
using System.Threading;
using boilerPlate.Controllers;
using boilerPlate.Infra.ServiceContracts;

namespace boilerPlate.BGServices
{
	public class DefaultBGService : BackgroundService
	{
        private readonly ILogger<DefaultBGService> _logger;
        IConfigService _config;
        public DefaultBGService(ILogger<DefaultBGService> logger, IConfigService config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical("Service Started!");
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical("Service Stopped!");
            return Task.CompletedTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogCritical($"Guided App Service Running:{DateTimeOffset.Now}");
                await Task.Delay(36000, stoppingToken);
            }
        }
    }
}

