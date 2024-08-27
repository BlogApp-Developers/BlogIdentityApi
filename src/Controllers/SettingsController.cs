namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.User.Models;

[Route("[controller]")]
public class SettingsController : Controller
{
    private readonly BlogIdentityDbContext dbContext;
    private readonly UserManager<User> userManager;

    public SettingsController(BlogIdentityDbContext dbContext, UserManager<User> userManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
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
}