using Azure;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
    {
        var orders = await orderInterface.GetAllAsync();
        if (!orders.Any())
            return NotFound("No orders found");

        var (_, list) = OrderConversion.FromEntity(null!, orders);
        return list!.Any() ? Ok(list) : NotFound("No orders found");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var order = await orderInterface.FindByIdAsync(id);
        if (order is null)
            return NotFound("Order not found");

        var (orderDTO, _) = OrderConversion.FromEntity(order, null!);
        return orderDTO is not null ? Ok(orderDTO) : NotFound("Order not found");
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
    {
        if (clientId <= 0)
            return BadRequest("Client ID is required");

        var orders = await orderService.GetOrdersByClientId(clientId);
        return orders.Any() ? Ok(orders) : NotFound("No orders found for client");
    }

    [HttpGet("details/{orderId:int}")]
    public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
    {
        if (orderId <= 0)
            return BadRequest("Order ID is required");

        var order = await orderService.GetOrderDetails(orderId);
        return order.OrderId > 0 ? Ok(order) : NotFound("Order not found");
    }

    [HttpPost]
    public async Task<ActionResult<Response>> CreateOrder(OrderDTO order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var getEntity = OrderConversion.ToEntity(order);
        var response = await orderInterface.CreateAsync(getEntity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    public async Task<ActionResult<Response>> UpdateOrder(OrderDTO order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var getEntity = OrderConversion.ToEntity(order);
        var response = await orderInterface.UpdateAsync(getEntity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    public async Task<ActionResult<Response>> DeleteOrder(OrderDTO order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var getEntity = OrderConversion.ToEntity(order);
        var response = await orderInterface.DeleteAsync(getEntity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }
}