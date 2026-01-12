using DesignPattern.StructuralPattern.Decorator.Restaurant.Decorator;

namespace DesignPattern.StructuralPattern.Decorator.Restaurant;

/// <summary>
/// ConcreteComponent - Origin drink 
/// </summary>
public  class Drink : IBeverage
{
    public int Id { get; set; }
    public string DrinkName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string GetDescription() => DrinkName;

    public decimal GetPrice() => Price;
}



