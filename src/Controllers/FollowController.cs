namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using BlogIdentityApi.Follow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.Follow.Repositories.Base;

[ApiController]
[Route("api/[controller]/[action]")]
public class FollowController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly IFollowRepository followRepository;

    public FollowController(UserManager<User> userManager, IUserRepository userRepository, IFollowRepository followRepository)
    {
        this.userManager = userManager;
        this.followRepository = followRepository;
    }

    [Authorize]
    [HttpPost("api/[controller]/[action]/{id}")]
    public async Task<IActionResult> Follow(Guid? id)
    {
        if (id.HasValue)
        {
            if (this.userManager.FindByIdAsync(id.ToString()) != null)
            {
                var followingUser = await this.userManager.GetUserAsync(base.User);
                var follow = new Follow(followingUser, id.Value);
                await followRepository.CreateAsync(follow);
                
                return Ok();
            }
        }
        return BadRequest();
    }

    [Authorize]
    [HttpPost("api/[controller]/[action]/{id}")]
    public async Task<IActionResult> Unfollow(Guid? id)
    {
        if (id.HasValue)
        {
            if (this.userManager.FindByIdAsync(id.ToString()) != null)
            {
                var follow = await this.followRepository.GetByIdAsync(id.Value);
                this.followRepository.DeleteAsync(follow);

                return base.NoContent();
            }
        }
        return BadRequest();
    }

    [Authorize]
    [HttpGet("api/[controller]/[action]")]
    public async Task<IActionResult> WhoToFollow(Guid? id)
    {
        throw new NotImplementedException();
    }
}