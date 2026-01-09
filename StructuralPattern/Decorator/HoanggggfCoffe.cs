namespace DesignPattern.StructuralPattern.Decorator;

public enum ProductType
{
    Coffee,
    Tea,
}

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public ProductType Type { get; set; }
}

public class Topping
{
    public int Id { get; set; }
    public string ToppingName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderTopping> OrderToppings { get; set; } = [];
}

public class OrderTopping
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ToppingId { get; set; }
    public Topping Topping { get; set; } = new Topping();
}

public interface IBeverage
{
    string  GetDescription();
    decimal GetPrice();
}

public class DatabaseBeverage(Product product) : IBeverage
{
    public string GetDescription() => product.ProductName;

    public decimal GetPrice() => product.BasePrice;
}

public class DecoratorBeverage(IBeverage beverage, Topping topping, DecoratorBeverage? core = null) : IBeverage
{
    public string GetDescription() => beverage.GetDescription() + $" {topping.ToppingName}";

    public decimal GetPrice() => beverage.GetPrice() + topping.Price;

    public DecoratorBeverage Decorator(IBeverage decorBeverage, Topping decorTopping)
    {
        return new DecoratorBeverage(decorBeverage, decorTopping, this);
    }
}

public class OrderService
{
    private static List<Product> _products = new List<Product>
    {
        new Product { Id = 1, ProductName = "Cà phê đen", BasePrice = 20000, Type = ProductType.Coffee },
        new Product { Id = 2, ProductName = "Cà phê sữa", BasePrice = 25000, Type = ProductType.Coffee },
        new Product { Id = 3, ProductName = "Trà sữa", BasePrice    = 30000, Type = ProductType.Tea }
    };

    private static List<Topping> _toppings = new List<Topping>
    {
        new Topping { Id = 1, ToppingName = "Sữa tươi", Price  = 5000, IsAvailable  = true },
        new Topping { Id = 2, ToppingName = "Đường", Price     = 2000, IsAvailable  = true },
        new Topping { Id = 3, ToppingName = "Kem tươi", Price  = 8000, IsAvailable  = true },
        new Topping { Id = 4, ToppingName = "Caramel", Price   = 7000, IsAvailable  = true },
        new Topping { Id = 5, ToppingName = "Trân châu", Price = 10000, IsAvailable = true },
        new Topping { Id = 6, ToppingName = "Chocolate", Price = 6000, IsAvailable  = false } // Hết hàng
    };

    private static List<Order> _orders         = [];
    private static int         _orderIdCounter = 1;

    public List<Product> GetAllProducts => _products;
    public List<Topping> GetAvailableToppings => _toppings.Where(x => x.IsAvailable).ToList();

    public Order GetOrderDetails(int orderId)
    {
        var result = _orders.FirstOrDefault(x => x.Id == orderId);
        return result ?? throw new Exception();
    }

    public Order CreateOrder(int productId, List<int> toppingIds)
    {
        var product = _products.FirstOrDefault(x => x.Id == productId);
        if (product is null)
        {
            throw new Exception();
        }

        IBeverage beverage         = new DatabaseBeverage(product);
        var       selectedToppings = new List<Topping>();
        foreach (var topping in toppingIds
            .Select(toppingId => GetAvailableToppings.FirstOrDefault(x => x.Id == toppingId)).OfType<Topping>())
        {
            beverage = new DecoratorBeverage(beverage, topping);
            selectedToppings.Add(topping);
        }

        var finalPrice       = beverage.GetPrice();
        var finalDescription = beverage.GetDescription();
        var order = new Order
        {
            Id = _orderIdCounter++, ProductName = product.ProductName, ProductId = productId, OrderDate = DateTime.Now,
            TotalPrice = finalPrice,
            OrderToppings = selectedToppings.Select(t => new OrderTopping { ToppingId = t.Id, Topping = t }).ToList()
        };
        _orders.Add(order);
        Console.WriteLine($"\n✓ Đã tạo order: {finalDescription}");
        Console.WriteLine($"✓ Tổng tiền: {finalPrice:N0} VNĐ");
        return order;
    }
}