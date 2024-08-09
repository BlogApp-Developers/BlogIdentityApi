namespace BlogIdentityApi.RefreshToken.Entity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using BlogIdentityApi.User.Models;

public class RefreshToken
{
    [NotNull]
    public Guid Token { get; set; }
    [ForeignKey("UserId"), NotNull]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
