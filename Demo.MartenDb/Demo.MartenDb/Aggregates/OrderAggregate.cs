using Demo.MartenDb.Events;
using Demo.MartenDb.Shared;

namespace Demo.MartenDb.Aggregates;

public record OrderAggregate(Guid Id, string OrderId, string Customer, List<Item> Items, int Version)
{
    public string Status { get; set; }
    public decimal? Total { get; set; }

    public static OrderAggregate Create(OrderCreatedEvent @event) =>
        new(@event.Id, @event.OrderId, @event.Customer, @event.Items, 1)
        {
            Status = OrderStatus.Created.ToString()
        };

    public OrderAggregate Apply(ItemAddedEvent @event)
    {
        if (!_canChangeOrder(Status))
            return this;

        Items.AddRange(@event.Items);
        return this with { Status = OrderStatus.ItemAdded.ToString(), Total = Items?.Sum(x => x.TotalItem()) };
    }

    public OrderAggregate Apply(OrderEventShippedEvent @event)
    {
        if (!_canChangeOrder(Status))
            return this;

        return this with { Status = OrderStatus.Shipped.ToString() };
    }

    public OrderAggregate Apply(OrderCancelledEvent @event)
    {
        if (!_canChangeOrder(Status))
            return this;

        return this with { Status = OrderStatus.Cancelled.ToString() };
    }

    private bool _canChangeOrder(string status)
       => _availableStatusForChange.Contains(Enum.Parse<OrderStatus>(status));

    private OrderStatus[] _availableStatusForChange
        => [OrderStatus.Created, OrderStatus.ItemAdded];
}