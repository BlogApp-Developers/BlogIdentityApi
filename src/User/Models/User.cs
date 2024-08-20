namespace BlogIdentityApi.User.Models;

using BlogIdentityApi.Follow.Models;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }
    public string? AboutMe { get; set; }
    public bool SendEmail { get; set; }
    public ICollection<Follow>? Followers { get; set; }
}
