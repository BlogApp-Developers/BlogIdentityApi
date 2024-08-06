namespace BlogIdentityApi.User.Models;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }
    public string? AboutMe { get; set; }
}
