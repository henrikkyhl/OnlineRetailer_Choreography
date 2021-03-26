using System.Collections.Generic;

namespace SharedModels
{
    public class OrderCreatedMessage
    {
        public int? CustomerId { get; set; }
        public int OrderId { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
    }
}
