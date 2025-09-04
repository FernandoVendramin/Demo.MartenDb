using Demo.MartenDb.Aggregates;
using Demo.MartenDb.Events;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace Demo.MartenDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IDocumentSession documentSession) : ControllerBase
    {
        [HttpPost("Create")]
        public async Task<ActionResult> Create(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            documentSession.Events.StartStream<OrderCreatedEvent>(@event.Id, @event);
            await documentSession.SaveChangesAsync(token: cancellationToken);

            return Created("api/Order/Create", @event.Id);
        }

        [HttpPut("AddItem")]
        public async Task<ActionResult> AddItem(ItemAddedEvent @event, CancellationToken cancellationToken)
        {
            documentSession.Events.Append(@event.Id, @event);
            await documentSession.SaveChangesAsync(token: cancellationToken);

            return Ok();
        }

        [HttpPut("Shipped")]
        public async Task<ActionResult> Shipped(OrderEventShippedEvent @event, CancellationToken cancellationToken)
        {
            documentSession.Events.Append(@event.Id, @event);
            await documentSession.SaveChangesAsync(token: cancellationToken);

            return Ok();
        }

        [HttpPut("Cancel")]
        public async Task<ActionResult> Cancel(OrderCancelledEvent @event, CancellationToken cancellationToken)
        {
            documentSession.Events.Append(@event.Id, @event);
            await documentSession.SaveChangesAsync(token: cancellationToken);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<OrderAggregate>> GetById(Guid id)
        {
            var aggregate = await documentSession.Events.AggregateStreamAsync<OrderAggregate>(id); // Agregado construido em tempo real
            return Ok(aggregate);
        }
    }
}
