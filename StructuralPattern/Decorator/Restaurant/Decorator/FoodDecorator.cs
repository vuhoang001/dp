namespace DesignPattern.StructuralPattern.Decorator.Restaurant.Decorator;

public static class BeverageExtensions
{
    public static IBeverage AddMilk(this IBeverage beverage, int quantity = 1)
        => new MilkDecorator(beverage, quantity);

    public static IBeverage AddIce(this IBeverage beverage, decimal price)
        => new IceDecorator(beverage, price);
}

public interface IBeverage
{
    public string  GetDescription();
    public decimal GetPrice();
}

/// <summary>
/// Decorator abstract - base class
/// </summary>
public abstract class BaseBeverage(IBeverage beverage) : IBeverage
{
    public virtual string  GetDescription() => beverage.GetDescription();
    public virtual decimal GetPrice()       => beverage.GetPrice();
}
public class MilkDecorator(IBeverage beverage, int quantity) : BaseBeverage(beverage)
{
    public override string GetDescription()
    {
        return base.GetDescription() + " Add Milk";
    }

    public override decimal GetPrice()
    {
        return base.GetPrice() + 200 * quantity;
    }
}

public class IceDecorator(IBeverage beverage, decimal price) : BaseBeverage(beverage)
{
    public override string GetDescription()
    {
        return base.GetDescription() + " Add Ice";
    }
    public override decimal GetPrice()
    {
        return base.GetPrice() + price;
    }
}