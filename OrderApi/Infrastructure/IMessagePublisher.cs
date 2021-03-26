using System.Collections.Generic;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderCreatedMessage(int? customerId, int orderId,
            IList<OrderLine> orderLines);
    }
}
