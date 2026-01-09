using System.Diagnostics;
using System.Linq;

namespace DesignPattern.BehaviourPattern.Cor.Solution.V2;


public static class ECommerceExample
{
    public static void Run()
    {
        Console.WriteLine("=======================================================================");
        Console.WriteLine("= Running Advanced E-Commerce Order Processing Chain-of-Responsibility =");
        Console.WriteLine("=======================================================================\n");

        // --- Setup Handlers and Decorators ---

        // 1. Main handlers for the core workflow
        var validationHandler = new OrderValidationHandler();
        var inventoryHandler = new InventoryCheckHandler();
        var fulfillmentHandler = new OrderFulfillmentHandler();

        // 2. Payment handler is special: it's a sub-chain of payment gateways
        var paymentHandler = new PaymentProcessingHandler()
            .AddGateway(new CreditCardGatewayHandler())
            .AddGateway(new PaypalGatewayHandler())
            .AddGateway(new CryptoGatewayHandler("BTC-Wallet-Address"));

        // 3. Notification handler, decorated with logging
        Handler<OrderContext> notificationHandler = new NotificationHandler();
        notificationHandler = new LoggingDecorator<OrderContext>(notificationHandler, "notification");

        // --- Assemble The Main Chain ---

        // 4. Decorate the core operational handlers (inventory and fulfillment)
        //    - Wrap them in a transaction.
        //    - Monitor their performance.
        var chainForDecoration = new Chain<OrderContext>()
                    .AddHandler(inventoryHandler)
                    .AddHandler(fulfillmentHandler);

        Handler<OrderContext> decoratedOps = new TransactionDecorator(
            new PerformanceMonitoringDecorator<OrderContext>(
                chainForDecoration.Build() // Important: Build returns the head of the chain for decoration
            ),
            chainForDecoration.GetHandlers()
        );


        // 5. Build the final chain
        var orderProcessingChain = new Chain<OrderContext>()
            .AddHandler(validationHandler)
            .AddHandler(paymentHandler)
            .AddHandler(decoratedOps) // Add the decorated sub-chain
            .AddHandler(notificationHandler);

        // --- Execute Scenarios ---

        // Scenario 1: Successful Order
        Console.WriteLine("--- SCENARIO 1: Successful Order ---\n");
        var successfulOrder = new Order
        {
            Customer = new Customer { Name = "Hoang Le", Email = "hoang.le@example.com" },
            Items = 
            [
                new OrderItem { ProductId = "PROD-001", Quantity = 1, UnitPrice = 999.99m },
                new OrderItem { ProductId = "PROD-002", Quantity = 2, UnitPrice = 49.50m }
            ],
            PaymentInfo = new PaymentInfo { Method = PaymentMethod.CreditCard, CardNumber = "1234-5678-9876-5432" }
        };
        var successContext = new OrderContext(successfulOrder);
        orderProcessingChain.Execute(successContext);
        PrintResult(successContext);


        // Scenario 2: Validation and Inventory Failure
        Console.WriteLine("\n--- SCENARIO 2: Validation & Inventory Failure ---\n");
        var failedOrder = new Order
        {
            Customer = new Customer { Name = "Anonymous", Email = "invalid-email" }, // Invalid email
            Items = 
            [
                // This item does not exist, will be caught by inventory check
                new OrderItem { ProductId = "PROD-999", Quantity = 1, UnitPrice = 10.0m } 
            ],
            PaymentInfo = new PaymentInfo { Method = PaymentMethod.PayPal, Email = "anon@paypal.com" }
        };
        var failureContext = new OrderContext(failedOrder);
        orderProcessingChain.Execute(failureContext);
        PrintResult(failureContext);
        
        // Scenario 3: Payment Failure (Credit Card fails, PayPal succeeds)
        Console.WriteLine("\n--- SCENARIO 3: Payment Failover (Credit Card -> PayPal) ---\n");
        var paymentFailoverOrder = new Order
        {
            Customer = new Customer { Name = "Charlie", Email = "charlie@example.com" },
            Items = 
            [
                new OrderItem { ProductId = "PROD-003", Quantity = 5, UnitPrice = 25.00m }
            ],
            PaymentInfo = new PaymentInfo { Method = PaymentMethod.CreditCard, CardNumber = "9999-9999-9999-9999" } // Invalid card
        };
        var paymentContext = new OrderContext(paymentFailoverOrder);
        orderProcessingChain.Execute(paymentContext);
        PrintResult(paymentContext);
    }
    
    private static void PrintResult(OrderContext context)
    {
        Console.WriteLine("\n--- ORDER PROCESSING RESULT ---");
        Console.WriteLine($"Order ID: {context.Order.Id}");
        Console.WriteLine($"Status: {(context.IsSuccessful ? "✅ SUCCESS" : "❌ FAILED")}");
        
        if (context.IsSuccessful)
        {
            Console.WriteLine($"Shipment ID: {context.Order.ShipmentId}");
            Console.WriteLine($"Notification Sent To: {context.Order.Customer.Email}");
        }
        else
        {
            Console.WriteLine("Errors:");
            foreach (var error in context.Errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }
        Console.WriteLine("-----------------------------\n");
    }
}


// =============================================================================
// 1. DTOs and Context Object
// =============================================================================

#region DTOs

public class OrderContext
{
    public Order Order { get; }
    public bool IsSuccessful { get; set; } = true;
    public List<string> Errors { get; } = [];
    
    // Using the same properties as Request<TData> for consistency
    public Dictionary<string, object> Context { get; set; } = new();
    private object? Result { get; set; }
    private Dictionary<string, object> Metadata { get; set; } = new();

    public OrderContext(Order order)
    {
        this.Order = order;
    }

    public void Fail(string message)
    {
        if(IsSuccessful) IsSuccessful = false;
        Errors.Add(message);
    }
    
    public void SetResult(object res) => this.Result = res;

    public T GetResult<T>()
    {
        if (Result is null) throw new InvalidOperationException("Result not set.");
        return (T)Result;
    }

    public void AddMetadata(string key, object value) => Metadata[key] = value;
    public bool TryGetMetadata(string key, out object value) => Metadata.TryGetValue(key, out value);
}


public class Order
{
    public string Id { get; } = $"ORD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public PaymentInfo PaymentInfo { get; set; }
    public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
    public string ShipmentId { get; set; }
}

public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PaymentInfo
{
    public PaymentMethod Method { get; set; }
    public string CardNumber { get; set; } 
    public string Email { get; set; }      
    public string WalletAddress { get; set; }
}

public enum PaymentMethod
{
    CreditCard,
    PayPal,
    Crypto
}

#endregion

// =============================================================================
// 2. Concrete Handlers for the Main Workflow
// =============================================================================

#region Handlers

public class OrderValidationHandler : Handler<OrderContext>
{
    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"[{Name}] Validating order {request.Data.Order.Id}...");
        var context = request.Data;

        if (context.Order.Customer == null)
            context.Fail("Customer information is missing.");
        
        if(context.Order.Customer != null && !context.Order.Customer.Email.Contains('@'))
            context.Fail($"Invalid customer email: {context.Order.Customer.Email}");

        if (context.Order.Items == null || !context.Order.Items.Any())
            context.Fail("Order must contain at least one item.");

        if (!context.IsSuccessful)
        {
            Console.WriteLine($"[{Name}] ❌ Validation failed.");
            return HandlerResult.Handled;
        }
        
        Console.WriteLine($"[{Name}] ✅ Validation passed.");
        return HandlerResult.Continue;
    }
}

public class InventoryCheckHandler : Handler<OrderContext>
{
    private static readonly Dictionary<string, int> Stock = new()
    {
        { "PROD-001", 10 }, { "PROD-002", 50 }, { "PROD-003", 20 }
    };
    
    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"[{Name}] Checking inventory...");
        var context = request.Data;

        foreach (var item in context.Order.Items)
        {
            if (!Stock.TryGetValue(item.ProductId, out var stock) || stock < item.Quantity)
            {
                context.Fail($"Product '{item.ProductId}' is out of stock or does not exist.");
            }
        }

        if (!context.IsSuccessful)
        {
            Console.WriteLine($"[{Name}] ❌ Inventory check failed.");
            return HandlerResult.Handled;
        }

        foreach (var item in context.Order.Items)
        {
            Stock[item.ProductId] -= item.Quantity;
        }
        context.AddMetadata("InventoryReserved", true);
        Console.WriteLine($"[{Name}] ✅ Inventory confirmed and reserved.");
        return HandlerResult.Continue;
    }
    
    public void Rollback(OrderContext context)
    {
        Console.WriteLine($"[{Name}] Reverting inventory reservation...");
        if (context.TryGetMetadata("InventoryReserved", out var reserved) && (bool)reserved)
        {
            foreach (var item in context.Order.Items)
            {
                if (Stock.ContainsKey(item.ProductId))
                {
                    Stock[item.ProductId] += item.Quantity;
                }
            }
        }
    }
}

public class OrderFulfillmentHandler : Handler<OrderContext>
{
    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"[{Name}] Preparing for fulfillment...");
        var context = request.Data;
        
        Thread.Sleep(150);
        context.Order.ShipmentId = $"SHP-{Guid.NewGuid().ToString().Substring(0, 12).ToUpper()}";
        
        Console.WriteLine($"[{Name}] ✅ Shipment {context.Order.ShipmentId} created.");
        return HandlerResult.Continue;
    }
}

public class NotificationHandler : Handler<OrderContext>
{
    public override HandlerResult Process(Request<OrderContext> request)
    {
        var context = request.Data;
        if (!context.IsSuccessful) return HandlerResult.Handled;

        Console.WriteLine($"[{Name}] Sending confirmation to {context.Order.Customer.Email}...");
        Thread.Sleep(100);
        Console.WriteLine($"[{Name}] ✅ Notification sent.");
        return HandlerResult.Handled;
    }
}

#endregion

// =============================================================================
// 3. Sub-Chain for Payment Processing
// =============================================================================

#region Payment Sub-Chain

public class PaymentProcessingHandler : Handler<OrderContext>
{
    private readonly Chain<OrderContext> _gatewayChain = new();

    public PaymentProcessingHandler AddGateway(Handler<OrderContext> gatewayHandler)
    {
        _gatewayChain.AddHandler(gatewayHandler);
        return this;
    }

    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"[{Name}] Starting payment processing for {request.Data.Order.TotalAmount:C}...");
        var context = request.Data;

        context.SetResult(false);
        _gatewayChain.Execute(request);

        if (context.GetResult<bool>() == false)
        {
            context.Fail("All payment gateways failed.");
            Console.WriteLine($"[{Name}] ❌ Payment failed.");
            return HandlerResult.Handled;
        }

        Console.WriteLine($"[{Name}] ✅ Payment successful.");
        return HandlerResult.Continue;
    }
}

public class CreditCardGatewayHandler : Handler<OrderContext>
{
    protected override bool CanHandle(Request<OrderContext> request) => request.Data.Order.PaymentInfo.Method == PaymentMethod.CreditCard;

    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"  -> [{Name}] Trying Credit Card...");
        var context = request.Data;
        
        if (context.Order.PaymentInfo.CardNumber.StartsWith("9999"))
        {
            Console.WriteLine($"  -> [{Name}] ❌ Card declined.");
            context.SetResult(false);
            return HandlerResult.Continue;
        }
        
        Thread.Sleep(200);
        Console.WriteLine($"  -> [{Name}] ✅ Payment approved.");
        context.SetResult(true);
        return HandlerResult.Handled;
    }
}

public class PaypalGatewayHandler : Handler<OrderContext>
{
    protected override bool CanHandle(Request<OrderContext> request)
    {
        var context = request.Data;
        bool isPrimary = context.Order.PaymentInfo.Method == PaymentMethod.PayPal;
        bool isFailover = false;
        try { isFailover = context.GetResult<bool>() == false; } catch (InvalidOperationException) {}

        return isPrimary || isFailover;
    }

    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"  -> [{Name}] Trying PayPal...");
        var context = request.Data;
        Thread.Sleep(250); // Corrected from Thread.Sleep
        Console.WriteLine($"  -> [{Name}] ✅ PayPal payment successful.");
        context.SetResult(true);
        return HandlerResult.Handled;
    }
}

public class CryptoGatewayHandler(string adminWallet) : Handler<OrderContext>
{
    protected override bool CanHandle(Request<OrderContext> request) => request.Data.Order.PaymentInfo.Method == PaymentMethod.Crypto;

    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine($"  -> [{Name}] Trying Crypto... Please send payment to {adminWallet}");
        request.Data.SetResult(true); 
        return HandlerResult.Handled;
    }
}

#endregion

// =============================================================================
// 4. Decorators for Cross-Cutting Concerns
// =============================================================================

#region Decorators

public abstract class BaseDecorator<TData>(Handler<TData> wrappedHandler) : Handler<TData>
{
    protected Handler<TData> _wrappedHandler = wrappedHandler;
    public override HandlerResult Process(Request<TData> request) => _wrappedHandler.Handle(request);
}

public class LoggingDecorator<TData>(Handler<TData> wrappedHandler, string tag = "") : BaseDecorator<TData>(wrappedHandler)
{
    public override HandlerResult Process(Request<TData> request)
    {
        var tagStr = string.IsNullOrEmpty(tag) ? "" : $" ({tag})";
        Console.WriteLine($"[LogDecorator{tagStr}] Before executing {_wrappedHandler.GetType().Name}...");
        var result = base.Process(request);
        Console.WriteLine($"[LogDecorator{tagStr}] After executing. Result: {result}");
        return result;
    }
}

public class PerformanceMonitoringDecorator<TData>(Handler<TData> wrappedHandler) : BaseDecorator<TData>(wrappedHandler)
{
    public override HandlerResult Process(Request<TData> request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = base.Process(request);
        stopwatch.Stop();
        Console.WriteLine($"[PerfMonitor] ⏱️ {_wrappedHandler.GetType().Name} executed in {stopwatch.ElapsedMilliseconds}ms.");
        return result;
    }
}

public class TransactionDecorator : BaseDecorator<OrderContext>
{
    private readonly IReadOnlyList<Handler<OrderContext>> _handlersInTransaction;

    public TransactionDecorator(Handler<OrderContext> wrappedHandler, IReadOnlyList<Handler<OrderContext>> handlers) : base(wrappedHandler)
    {
        _handlersInTransaction = handlers;
    }
    
    public override HandlerResult Process(Request<OrderContext> request)
    {
        Console.WriteLine("[Transaction] Begin transaction...");
        
        var context = request.Data;
        var result = base.Process(request);

        if (result == HandlerResult.Handled && !context.IsSuccessful)
        {
            Rollback(context);
            return HandlerResult.Handled;
        }

        Console.WriteLine("[Transaction] ✅ Commit transaction.");
        return result;
    }

    private void Rollback(OrderContext context)
    {
        Console.WriteLine("[Transaction] 롤백 (Rollback) initiated due to handler failure.");
        
        var inventoryHandler = _handlersInTransaction.OfType<InventoryCheckHandler>().FirstOrDefault();
        inventoryHandler?.Rollback(context);
    }
}

#endregion
