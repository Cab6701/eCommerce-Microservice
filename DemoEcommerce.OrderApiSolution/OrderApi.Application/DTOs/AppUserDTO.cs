using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record struct AppUserDTO(
        int Id,
        [Required] string Username,
        [Required] string TelephoneNumber,
        [Required] string Address,
        [Required, EmailAddress] string Email,
        [Required] string Password,
        [Required] string Role
    );
}