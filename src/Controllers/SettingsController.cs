namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;

[Route("api/[controller]/[action]")]
public class SettingsController : ControllerBase
{
    private readonly BlogIdentityDbContext dbContext;
    private readonly UserManager<User> userManager;
    private readonly IUserRepository userRepository;

    public SettingsController(BlogIdentityDbContext dbContext, UserManager<User> userManager, IUserRepository userRepository)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.userRepository = userRepository;
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> ChangeEmailSend(bool toSend)
    {
        try
        {
            var user = await this.userManager.GetUserAsync(base.User);
            user.SendEmail = toSend;

            this.dbContext.Users.Update(user);
            this.dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            return base.BadRequest(ex.Message);
        }

        return base.NoContent();
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> EditProfile(User updatedUser)
    {
        try
        {
            this.dbContext.Users.Update(updatedUser);
            this.dbContext.SaveChanges();
            await this.userRepository.UpdateAsync(updatedUser);
        }
        catch (Exception ex)
        {
            return base.BadRequest(ex.Message);
        }

        return base.Ok();
    }
}