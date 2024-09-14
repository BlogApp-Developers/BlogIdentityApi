namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using BlogIdentityApi.Follow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.Follow.Repositories.Base;
using BlogIdentityApi.Data;

[ApiController]
[Route("/api/[controller]/[action]")]
public class FollowController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly IFollowRepository followRepository;
    private readonly IUserRepository userRepository;
    private readonly BlogIdentityDbContext dbContext;

    public FollowController(UserManager<User> userManager, IUserRepository userRepository, IFollowRepository followRepository, BlogIdentityDbContext dbContext)
    {
        this.userManager = userManager;
        this.followRepository = followRepository;
        this.userRepository = userRepository;
        this.dbContext = dbContext;
    }

    [Authorize]
    [HttpPost("/api/[controller]/[action]/{id}")]
    public async Task<IActionResult> Follow(Guid? id)
    {
        if (id.HasValue)
        {
            var followingUser = await this.userManager.GetUserAsync(base.User);
            var follow = new Follow(followingUser, id.Value);
            await followRepository.CreateAsync(follow);
                
            return Ok();
            
        }
        return BadRequest();
    }

    [Authorize]
    [HttpPost("/api/[controller]/[action]/{id}")]
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
    [HttpGet("/api/[controller]/[action]")]
    public async Task<IActionResult> WhoToFollow()
    {
        var user = await this.userManager.GetUserAsync(base.User);

        if (user == null)
        {
            return base.Forbid();
        }

        var users = await this.userRepository.GetFiveRandomThroughTopics(user.Id);
        return base.Ok(users);
    }

    [Authorize]
    [HttpGet("/api/[controller]/[action]/{id}")]
    public async Task<IActionResult> GetFollowersCount(Guid? id)
    {
        if (id.HasValue)
        {
            var followers = await this.followRepository.GetsByIdAsync(id.Value);

            return base.Ok(followers.Count());
        }
        else
        {
            return base.BadRequest();
        }
    }

    [Authorize]
    [HttpGet("/api/[controller]/[action]/{id}")]
    public async Task<IActionResult> GetFollowingCount(Guid? id)
    {
        if (id.HasValue)
        {
            var following = await this.followRepository.GetsInvertByIdAsync(id.Value);

            return base.Ok(following.Count());
        }
        else
        {
            return base.BadRequest();
        }
    }

    [Authorize]
    [HttpGet("/api/[controller]/[action]/{id}")]
    public async Task<IActionResult> GetFollowers(Guid? id)
    {
        if (id.HasValue)
        {
            var followersAccounts = await this.followRepository.GetFollowersByIdAsync(id.Value);

            return Ok(followersAccounts);
        }
        else
        {
            return base.BadRequest();
        }
    }

    [Authorize]
    [HttpGet("/api/[controller]/[action]/{id}")]
    public async Task<IActionResult> GetFollowings(Guid? id)
    {
        if (id.HasValue)
        {
            var followingsAccounts = await this.followRepository.GetFollowingsByIdAsync(id.Value);

            return Ok(followingsAccounts);
        }
        else
        {
            return base.BadRequest();
        }
    }
}