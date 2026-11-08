using NUnit.Framework;
using NSubstitute;
using Microsoft.Extensions.Logging;
using CarFactoryApp.Services;
using CarFactoryApp.Models;

namespace CarFactoryApp.Tests;

[TestFixture]
public class CarFactoryTests
{
    [Test]
    public void BuildCar_ShouldAssembleCarWithAllComponentsFromFactories()
    {
        // ARRANGE (Styl londyński - tworzenie izolowanych Test Doubles z NSubstitute)
        
        // Stubs: zdefiniowane zwracane wartości komponentów
        var engineFactoryStub = Substitute.For<IEngineFactory>();
        var dummyMetal = new Metal("Stal", 100);
        var expectedEngine = new Engine("V6-STUB", 300, dummyMetal);
        engineFactoryStub.BuildEngine().Returns(expectedEngine);

        var wheelFactoryStub = Substitute.For<IWheelFactory>();
        var expectedWheels = new List<Wheel>
        {
            new(18, "Grip"), new(18, "Grip"), new(18, "Grip"), new(18, "Grip")
        };
        wheelFactoryStub.BuildWheels(4).Returns(expectedWheels);

        // Stub: obiekt do weryfikacji interakcji oraz dostarczenia wartości
        var bodyFactoryStub = Substitute.For<IBodyFactory>();
        var expectedBody = new Body("Czerwony", "Sport", dummyMetal);
        bodyFactoryStub.BuildBody("Czerwony").Returns(expectedBody);

        // Dummy: logger wymagany przez konstruktor
        var loggerDummy = Substitute.For<ILogger<CarFactory>>();

        // Obiekt testowany (System Under Test - SUT)
        var carFactory = new CarFactory(
            engineFactoryStub, 
            wheelFactoryStub, 
            bodyFactoryStub, 
            loggerDummy);

        // ACT
        var resultCar = carFactory.BuildCar("SuperCar", "Czerwony");

        // ASSERT
        // 1. Weryfikacja stanu (state verification)
        Assert.That(resultCar, Is.Not.Null);
        Assert.That(resultCar.Model, Is.EqualTo("SuperCar"));
        Assert.That(resultCar.Engine, Is.EqualTo(expectedEngine));
        Assert.That(resultCar.Wheels.Count, Is.EqualTo(4));
        Assert.That(resultCar.Body, Is.EqualTo(expectedBody));

        // 2. Weryfikacja interakcji w stylu londyńskim (behavior verification)
        engineFactoryStub.Received(1).BuildEngine();
        wheelFactoryStub.Received(1).BuildWheels(4);
        bodyFactoryStub.Received(1).BuildBody("Czerwony");
    }

    [Test]
    public void EngineFactory_BuildEngine_ShouldRequestMetalFromMetalProvider()
    {
        // ARRANGE - Testowanie niższej warstwy fabryki (EngineFactory) w izolacji
        var metalProviderStub = Substitute.For<IMetalProvider>();
        var expectedMetal = new Metal("Stal Tytanowa", 150.0);
        metalProviderStub.GetMetal(150.0).Returns(expectedMetal);

        var loggerDummy = Substitute.For<ILogger<EngineFactory>>();

        var engineFactory = new EngineFactory(metalProviderStub, loggerDummy);

        // ACT
        var engine = engineFactory.BuildEngine();

        // ASSERT
        Assert.That(engine.UsedMetal, Is.EqualTo(expectedMetal));
        
        // Weryfikacja, że EngineFactory poprawnie odwołał się do IMetalProvider z wagą 150kg
        metalProviderStub.Received(1).GetMetal(150.0);
    }
}
