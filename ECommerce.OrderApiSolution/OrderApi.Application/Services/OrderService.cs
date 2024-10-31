using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Mappers;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services;

public class OrderService(IOrder order, HttpClient httpClient,
    ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
{
    public async Task<ProductDto> GetProduct(int productId)
    {
        // Call product Api using HttpClient
        // Redirect this call to the API Gateway since product Api is not response to outsiders.

        var getProduct = await httpClient.GetAsync($"/api/Products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
            return null!;

        var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();
        return product!;
    }

    public async Task<AppUserDto> GetUser(int userId)
    {
        // Call product Api using HttpClient
        // Redirect this call to the API Gateway since product Api is not response to outsiders.

        var getUser = await httpClient.GetAsync($"/api/Authentication/getUser/{userId}");
        if (!getUser.IsSuccessStatusCode)
            return null!;

        var user = await getUser.Content.ReadFromJsonAsync<AppUserDto>();
        return user!;
    }


    public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
    {
        // prepare order
        var currentOrder = await order.FindByIdAsync(orderId);
        if (currentOrder is null || currentOrder!.Id <= 0) return null!;

        // Get Retry pipeline
        var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

        // Prepare product
        var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(currentOrder.ProductId));


        // Prepare Client
        var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(currentOrder.ClientId));

        // Populate order details
        return new OrderDetailsDto(
            currentOrder.Id,
            productDto.Id,
            appUserDto.Id,
            appUserDto.Name,
            appUserDto.Email,
            appUserDto.Address,
            appUserDto.TelephoneNumber,
            productDto.Name,
            currentOrder.PurchaseQuantity,
            (int)productDto.Price,
            productDto.Quantity * currentOrder.PurchaseQuantity,
            currentOrder.OrderDate
            );
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
    {
        var orders = await order.GetOrdersAsync(o => o.ClientId.Equals(clientId));
        if (orders is null || !orders.Any()) return null!;

        var (_, _orders) = OrderMappers.ToDto(null, orders);
        return _orders!;
    }
}
