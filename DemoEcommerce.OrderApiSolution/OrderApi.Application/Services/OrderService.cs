using System.Net.Http.Json;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;

namespace OrderApi.Application.Services;
public class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
{
    public async Task<ProductDTO> GetProduct(int productId)
    {
        var getProduct = await httpClient.GetAsync($"api/products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get product {productId}");
        }

        var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product!;
    }

    public async Task<AppUserDTO> GetUser(int userId)
    {
        var getUser = await httpClient.GetAsync($"api/users/{userId}");
        if (!getUser.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get user {userId}");
        }

        var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user!;
    }

    public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
        var getOrder = await orderInterface.FindByIdAsync(orderId);
        if (getOrder is null || getOrder!.Id <= 0)
        {
            throw new Exception($"Order {orderId} not found");
        }

        // Get Retry pipeline
        var retryPipeline = resiliencePipeline.GetPipeline("Retry");

        // Prepare product
        var productDTO = await retryPipeline.ExecuteAsync<ProductDTO>(async token => await GetProduct(getOrder.ProductId));

        // Prepare user
        var userDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(getOrder.ClientId));

        // Populate order details
        var orderDetails = new OrderDetailsDTO(
            getOrder.Id,
            productDTO.Id,
            userDTO.Id,
            userDTO.Username,
            userDTO.Email,
            userDTO.Address,
            userDTO.TelephoneNumber,
            productDTO.Name,
            getOrder.PurchasedQuantity,
            productDTO.Price,
            productDTO.Price * getOrder.PurchasedQuantity,
            getOrder.OrderedDate
        );

        return orderDetails;
    }

    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
    {
        var getOrders = await orderInterface.GetOrdersAsync(order => order.ClientId == clientId);
        if (!getOrders.Any())
        {
            throw new Exception($"No orders found for client {clientId}");
        }

        // Convert to DTO
        var (_, orders) = OrderConversion.FromEntity(null!, getOrders);
        return orders!;
    }
}