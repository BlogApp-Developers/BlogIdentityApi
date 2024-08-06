namespace BlogIdentityApi.RefreshToken.Entity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogIdentityApi.User.Models;

public class RefreshToken
{
    [Required]
    public Guid Token { get; set; }
    [ForeignKey("UserId"), Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
