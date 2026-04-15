using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record struct OrderDTO(
        int Id,
        [Required, Range(1, int.MaxValue)] int ProductId,
        [Required, Range(1, int.MaxValue)] int ClientId,
        [Required, Range(1, int.MaxValue)] int PurchasedQuantity,
        DateTime OrderedDate
    );
}