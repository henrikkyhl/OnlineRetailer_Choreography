using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using SharedModels;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        IOrderRepository repository;
        IMessagePublisher messagePublisher;

        public OrdersController(IRepository<Order> repos,
            IMessagePublisher publisher)
        {
            repository = repos as IOrderRepository;
            messagePublisher = publisher;
        }

        // GET orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody]Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            try
            {
                // Create a tentative order.
                order.Status = Order.OrderStatus.tentative;
                var newOrder = repository.Add(order);

                // Publish OrderStatusChangedMessage. 
                messagePublisher.PublishOrderCreatedMessage(
                    newOrder.customerId, newOrder.Id, newOrder.OrderLines);


                // Wait until order status is "completed"
                bool completed = false;
                while (!completed)
                {
                    var tentativeOrder = repository.Get(newOrder.Id);
                    if (tentativeOrder.Status == Order.OrderStatus.completed)
                        completed = true;
                    Thread.Sleep(100);
                }

                return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
            }
            catch
            {
                return StatusCode(500, "An error happened. Try again.");
            }

        }


        // PUT orders/5/cancel
        // This action method cancels an order and publishes an OrderStatusChangedMessage
        // with topic set to "cancelled".
        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            throw new NotImplementedException();

            // Add code to implement this method.
        }

        // PUT orders/5/ship
        // This action method ships an order and publishes an OrderStatusChangedMessage.
        // with topic set to "shipped".
        [HttpPut("{id}/ship")]
        public IActionResult Ship(int id)
        {
            throw new NotImplementedException();

            // Add code to implement this method.
        }

        // PUT orders/5/pay
        // This action method marks an order as paid and publishes a CreditStandingChangedMessage
        // (which have not yet been implemented), if the credit standing changes.
        [HttpPut("{id}/pay")]
        public IActionResult Pay(int id)
        {
            throw new NotImplementedException();

            // Add code to implement this method.
        }

    }
}
