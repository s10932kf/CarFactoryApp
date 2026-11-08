using Microsoft.Extensions.Logging;
using CarFactoryApp.Models;

namespace CarFactoryApp.Services;

// 1. Dostawca surowca
public interface IMetalProvider
{
    Metal GetMetal(double amountKg);
}

public class MetalProvider : IMetalProvider
{
    private readonly ILogger<MetalProvider> _logger;

    public MetalProvider(ILogger<MetalProvider> logger)
    {
        _logger = logger;
    }

    public Metal GetMetal(double amountKg)
    {
        _logger.LogInformation("Pozyskiwanie surowca: {Amount} kg metalu.", amountKg);
        return new Metal("Stal ocynkowana", amountKg);
    }
}

// 2. Fabryka Silników
public interface IEngineFactory
{
    Engine BuildEngine();
}

public class EngineFactory : IEngineFactory
{
    private readonly IMetalProvider _metalProvider;
    private readonly ILogger<EngineFactory> _logger;

    public EngineFactory(IMetalProvider metalProvider, ILogger<EngineFactory> logger)
    {
        _metalProvider = metalProvider;
        _logger = logger;
    }

    public Engine BuildEngine()
    {
        _logger.LogInformation("Rozpoczynanie produkcji silnika V6...");
        var metal = _metalProvider.GetMetal(150.0);
        _logger.LogInformation("Odlewanie bloku silnika z pozyskanego metalu...");
        return new Engine("V6-TURBO", 350, metal);
    }
}

// 3. Fabryka Kół
public interface IWheelFactory
{
    List<Wheel> BuildWheels(int count);
}

public class WheelFactory : IWheelFactory
{
    private readonly ILogger<WheelFactory> _logger;

    public WheelFactory(ILogger<WheelFactory> logger)
    {
        _logger = logger;
    }

    public List<Wheel> BuildWheels(int count)
    {
        _logger.LogInformation("Produkcja zestawe kół ({Count} szt.)...", count);
        var wheels = new List<Wheel>();
        for (int i = 0; i < count; i++)
        {
            wheels.Add(new Wheel(18, "Premium Grip"));
        }
        return wheels;
    }
}

// 4. Fabryka Karoserii
public interface IBodyFactory
{
    Body BuildBody(string color);
}

public class BodyFactory : IBodyFactory
{
    private readonly IMetalProvider _metalProvider;
    private readonly ILogger<BodyFactory> _logger;

    public BodyFactory(IMetalProvider metalProvider, ILogger<BodyFactory> logger)
    {
        _metalProvider = metalProvider;
        _logger = logger;
    }

    public Body BuildBody(string color)
    {
        _logger.LogInformation("Formowanie karoserii w kolorze: {Color}...", color);
        var metal = _metalProvider.GetMetal(300.0);
        return new Body(color, "Sedan", metal);
    }
}

// 5. Główna Fabryka Samochodów
public interface ICarFactory
{
    Car BuildCar(string model, string color);
}

public class CarFactory : ICarFactory
{
    private readonly IEngineFactory _engineFactory;
    private readonly IWheelFactory _wheelFactory;
    private readonly IBodyFactory _bodyFactory;
    private readonly ILogger<CarFactory> _logger;

    public CarFactory(
        IEngineFactory engineFactory,
        IWheelFactory wheelFactory,
        IBodyFactory bodyFactory,
        ILogger<CarFactory> logger)
    {
        _engineFactory = engineFactory;
        _wheelFactory = wheelFactory;
        _bodyFactory = bodyFactory;
        _logger = logger;
    }

    public Car BuildCar(string model, string color)
    {
        _logger.LogInformation("=== Rozpoczynanie montażu samochodu: {Model} ===", model);
        
        var engine = _engineFactory.BuildEngine();
        var wheels = _wheelFactory.BuildWheels(4);
        var body = _bodyFactory.BuildBody(color);

        _logger.LogInformation("Końcowy montaż podzespołów w całość dla modelu {Model}.", model);
        return new Car(model, engine, wheels, body);
    }
}
