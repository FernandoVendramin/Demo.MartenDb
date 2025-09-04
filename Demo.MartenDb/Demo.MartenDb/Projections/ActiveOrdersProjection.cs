using Demo.MartenDb.Events;
using Demo.MartenDb.Shared;
using Marten.Events.Aggregation;
using Marten.Schema.Identity;

namespace Demo.MartenDb.Projections;

public record ActiveOrdersSummary(Guid Id, string OrderId, string Customer, decimal Total, string Status, List<Item> Items);

public class ActiveOrdersProjection : SingleStreamProjection<ActiveOrdersSummary>
{
    public ActiveOrdersSummary Create(OrderCreatedEvent @event)
        => new(CombGuidIdGeneration.NewGuid(), @event.OrderId, @event.Customer, @event.Items.Sum(x => x.TotalItem()), OrderStatus.Created.ToString(), @event.Items);

    public ActiveOrdersSummary Apply(ItemAddedEvent @event, ActiveOrdersSummary current)
    {
        if (!_canChangeOrder(current.Status))
            return current;

        current.Items.AddRange(@event.Items);
        return current with { Status = OrderStatus.ItemAdded.ToString(), Total = current.Total + @event.Items.Sum(x => x.TotalItem()) };
    }

    public ActiveOrdersSummary Apply(OrderEventShippedEvent @event, ActiveOrdersSummary current)
    {
        if (!_canChangeOrder(current.Status))
            return current;

        return current with { Status = OrderStatus.Shipped.ToString() };
    }

    public ActiveOrdersSummary Apply(OrderCancelledEvent @event, ActiveOrdersSummary current)
    {
        if (!_canChangeOrder(current.Status))
            return current;

        return current with { Status = OrderStatus.Cancelled.ToString() };
    }

    private bool _canChangeOrder(string status)
        => _availableStatusForChange.Contains(Enum.Parse<OrderStatus>(status));

    private OrderStatus[] _availableStatusForChange
        => new OrderStatus[] { OrderStatus.Created, OrderStatus.ItemAdded };
}