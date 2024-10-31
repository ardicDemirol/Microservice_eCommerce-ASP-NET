using ECommerce.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Mappers;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController(IOrder order, IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await order.GetAllAsync();
        if (!orders.Any()) return NotFound("No order detected in the dataase");

        var (_, list) = OrderMappers.ToDto(null, orders);
        return !list!.Any() ? NotFound("No order detected in the dataase") : Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var currentOrder = await order.FindByIdAsync(id);
        if (currentOrder is null) return NotFound("Order not found");

        var (_order, _) = OrderMappers.ToDto(currentOrder, null);
        return Ok(_order);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<OrderDto>> GetClientOrders(int clientId)
    {
        if (clientId <= 0) return BadRequest("Invalid client id");

        var orders = await orderService.GetOrdersByClientId(clientId);
        if (!orders.Any()) return NotFound("No order detected in the dataase");

        return !orders!.Any() ? NotFound("No order detected in the dataase") : Ok(orders);
    }

    [HttpGet("details/{orderId:int}")]
    public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int orderId)
    {
        if (orderId <= 0) return BadRequest("Invalid order id");

        var orderDetails = await orderService.GetOrderDetails(orderId);
        return orderDetails is null ? NotFound("Order details not found") : Ok(orderDetails);
    }


    [HttpPost]
    public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid order details");

        var orderEntity = OrderMappers.ToEntity(orderDto);
        var response = await order.CreateAsync(orderEntity);
        return response.Flag ? Ok(response) : BadRequest(response);

    }

    [HttpPut]
    public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid order details");

        var orderEntity = OrderMappers.ToEntity(orderDto);
        var response = await order.UpdateAsync(orderEntity);
        return response.Flag ? Ok(response) : BadRequest(response);

    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Response>> DeleteOrder(OrderDto orderDto)
    {
        var currentOrder = OrderMappers.ToEntity(orderDto);
        var response = await order.DeleteAsync(currentOrder);

        return response.Flag ? Ok(response) : BadRequest(response);
    }




}
