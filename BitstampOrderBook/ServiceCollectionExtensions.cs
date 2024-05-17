using BitstampOrderBook.Data;
using BitstampOrderBook.Data.Services;
using BitstampOrderBook.Data.Services.WebSocketServices;
using Microsoft.EntityFrameworkCore;

namespace BitstampOrderBook
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<OrderBookService>();
            services.AddScoped<WebSocketService>();
            services.AddHostedService<WebSocketBackgroundService>();

            services.AddSignalR();

            services.AddLogging(logging =>
            {
                logging.AddConsole();
            });

            return services;
        }
    }
}
