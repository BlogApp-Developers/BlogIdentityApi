namespace BlogIdentityApi.Follow.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using BlogIdentityApi.User.Models;

public class Follow
{
    public Follow(User followingUser, Guid id)
    {
        this.Following = followingUser;
        this.FollowingId = followingUser.Id;
        this.FollowerId = id;
    }

    public Follow() {}

    [Key]
    public Guid Id { get; set; }
    [NotNull]
    public Guid FollowerId { get; set; }
    public User? Follower { get; set; }
    [ForeignKey(name: "FollowingId"), NotNull]
    public Guid? FollowingId { get; set; }
    public User? Following { get; set; }
}
