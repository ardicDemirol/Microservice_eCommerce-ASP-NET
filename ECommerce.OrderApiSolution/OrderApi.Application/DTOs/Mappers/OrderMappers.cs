using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Mappers;

public static class OrderMappers
{
    public static Order ToEntity(this OrderDto orderDto) => new()
    {
        Id = orderDto.Id,
        ProductId = orderDto.ProductId,
        ClientId = orderDto.ClientId,
        PurchaseQuantity = orderDto.PurchaseQuantity,
        OrderDate = orderDto.OrderDate
    };

    public static (OrderDto?, IEnumerable<OrderDto>?) ToDto(Order? order, IEnumerable<Order>? orders)
    {
        if (order is not null || orders is null)
        {
            var singleOrder = new OrderDto(
                order!.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderDate
                );
            return (singleOrder, null);
        }

        if (order is null || orders is not null)
        {
            var orderList = orders!.Select(order => new OrderDto(
                order.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderDate
                ));
            return (null, orderList);
        }

        return (null, null);
    }



}
