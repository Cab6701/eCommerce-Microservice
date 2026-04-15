using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO order) => new()
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ClientId = order.ClientId,
            PurchasedQuantity = order.PurchasedQuantity,
            OrderedDate = order.OrderedDate
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order order, IEnumerable<Order?> orders)
        {
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(order!.Id, order.ProductId, order.ClientId, order.PurchasedQuantity, order.OrderedDate);
                return (singleOrder, null);
            }

            if (orders is not null || order is null)
            {
                var _orders = orders!.Select(o => new OrderDTO(o!.Id, o.ProductId, o.ClientId, o.PurchasedQuantity, o.OrderedDate)).ToList();
                return (null, _orders);
            }

            return (null, null);
        }
    }
}