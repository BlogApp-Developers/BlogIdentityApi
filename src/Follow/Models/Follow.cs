namespace BlogIdentityApi.Follow.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using BlogIdentityApi.User.Models;

public class Follow
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(name: "FollowerId"), NotNull]
    public required Guid? FollowingId { get; set; }
    public User? Following { get; set; }
}
