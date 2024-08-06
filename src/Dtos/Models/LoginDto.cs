namespace BlogIdentityApi.Dtos.Models;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Email { get; set; }

}
