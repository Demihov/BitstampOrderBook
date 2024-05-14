namespace BitstampOrderBook.Data.Services
{
    public class WebSocketBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<WebSocketBackgroundService> _logger;

        public WebSocketBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<WebSocketBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var uri = new Uri("wss://ws.bitstamp.net");

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
                await webSocketService.ConnectAsync(uri, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting WebSocket connection.");
            }
        }
    }
}
