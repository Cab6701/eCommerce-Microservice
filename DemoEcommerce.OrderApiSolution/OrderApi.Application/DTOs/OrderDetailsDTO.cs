using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record struct OrderDetailsDTO(
        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int ClientId,
        [Required] string ClientName,
        [Required, EmailAddress] string Email,
        [Required] string Address,
        [Required] string TelephoneNumber,
        [Required] string ProductName,
        [Required] int PurchasedQuantity,
        [Required, DataType(DataType.Currency)] decimal? UnitPrice,
        [Required, DataType(DataType.Currency)] decimal? TotalPrice,
        [Required] DateTime OrderedDate
    );
}