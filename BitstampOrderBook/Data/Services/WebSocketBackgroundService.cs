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

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var uri = new Uri("wss://ws.bitstamp.net");

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
                await webSocketService.ConnectAsync(uri, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting WebSocket connection.");
            }
        }
    }
}
