using BuyMyHouse.Infrastructure;
using BuyMyHouse.Services;
using BuyMyHouse.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunctionApp1
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(Configure)
                .Build();

            host.Run();
        }

        static void Configure(HostBuilderContext Builder, IServiceCollection Services)
        {
            Services.AddSingleton<BlobStorage>();
            Services.AddSingleton<QueStorage>();
            Services.AddSingleton<TableStorageUser>();
            Services.AddSingleton<TableStorageHouse>();
            Services.AddScoped<IUserService, UserService>();
            Services.AddScoped<IHouseService, HouseService>();
        }
    }
}