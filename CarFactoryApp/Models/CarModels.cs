namespace CarFactoryApp.Models;

public record Metal(string Type, double WeightKg);
public record Engine(string Code, int HorsePower, Metal UsedMetal);
public record Wheel(int SizeInches, string Brand);
public record Body(string Color, string Style, Metal UsedMetal);
public record Car(string Model, Engine Engine, List<Wheel> Wheels, Body Body);