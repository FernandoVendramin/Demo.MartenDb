namespace Demo.MartenDb.Events;

public record Item(int Quantity, decimal Price)
{
    public decimal TotalItem() => Quantity * Price;
}

public record OrderCreatedEvent(Guid Id, string OrderId, string Customer, List<Item> Items);

public record ItemAddedEvent(Guid Id, List<Item> Items);

public record OrderEventShippedEvent(Guid Id);

public record OrderCancelledEvent(Guid Id);
