namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;

[Route("[controller]")]
public class SettingsController : Controller
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
    [HttpPut("api/[controller]/[action]")]
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
    [HttpPut("api/[controller]/[action]")]
    public async Task<IActionResult> EditProfile(User updatedUser)
    {
        try
        {
            var user = await this.userManager.GetUserAsync(base.User);
            user = updatedUser;

            this.dbContext.Users.Update(user);
            this.dbContext.SaveChanges();
            await this.userRepository.UpdateAsync(user);
            
        }
        catch (Exception ex)
        {
            return base.BadRequest(ex.Message);
        }

        return base.Ok();
    }
}