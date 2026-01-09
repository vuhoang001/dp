using DesignPattern.StructuralPattern.Composite;

namespace DesignPattern.BehaviourPattern.Cor.Solution;

public class PurchaseOrder
{
    public int Id { get; set; }
    public string DocCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<PurchaseOrderLine> PurchaseOrderLines = [];
}

public class PurchaseOrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}


public class PurchaseOrderValidation : Handler<PurchaseOrder>
{
    private List<Item> items =
    [
        new Item
        {
            Id       = 1,
            Name     = "Prod1",
            ParentId = null
        },
        new Item
        {
            Id       = 2,
            Name     = "Prod2",
            ParentId = null
        },
        new Item
        {
            Id       = 3,
            Name     = "Prod3",
            ParentId = null
        }
    ];

    public override HandlerResult Process(Request<PurchaseOrder> request)
    {
        var order = request.Data;

        if (!string.IsNullOrWhiteSpace(order.DocCode) || order.TotalAmount <= 0) return HandlerResult.Handled;
        var productsName      = items.Select(x => x.Name);
        var isNotExistProduct = order.PurchaseOrderLines.Any(x => !productsName.Contains(x.ProductName));
        return isNotExistProduct ? throw new Exception("hahahahahah") : HandlerResult.Continue;
    }
}