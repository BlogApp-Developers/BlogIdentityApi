namespace BlogIdentityApi.Dtos.Models;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required]
    public string? Email { get; set; }

}
