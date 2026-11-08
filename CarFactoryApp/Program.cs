using CarFactoryApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (_, services) =>
        {
            // Rejestracja serwisów w kontenerze Dependency Injection
            services.AddTransient<IMetalProvider, MetalProvider>();
            services.AddTransient<IEngineFactory, EngineFactory>();
            services.AddTransient<IWheelFactory, WheelFactory>();
            services.AddTransient<IBodyFactory, BodyFactory>();
            services.AddTransient<ICarFactory, CarFactory>();
        }
    )
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

// Pobranie głównego serwisu fabryki z kontenera DI
var carFactory = host.Services.GetRequiredService<ICarFactory>();
var car = carFactory.BuildCar("Model GT", "Czarny Metallik");

Console.WriteLine(
    $"Wyprodukowano auto: {car.Model} z silnikiem {car.Engine.Code} ({car.Engine.HorsePower} KM) oraz {car.Wheels.Count} kołami."
);
