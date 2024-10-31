using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs;

public record OrderDetailsDto(
    [Required] int OrderId,
    [Required] int ProductId,
    [Required] int Client,
    [Required] string Name,
    [Required, EmailAddress] string Email,
    [Required, EmailAddress] string Address,
    [Required] string TelephoneNumber,
    [Required] string ProductName,
    [Required] int PurchaseQuantity,
    [Required, DataType(DataType.Currency)] int UnitPrice,
    [Required, DataType(DataType.Currency)] int TotalPrice,
    [Required] DateTime OrderDate
    );
