namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using Azure.Storage.Blobs;
using BlogIdentityApi.Dtos.Models;

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
    public async Task<IActionResult> EditProfile(UpdateDto updatedUser, IFormFile avatar)
    {
        try
        {
            var user = await this.userManager.GetUserAsync(base.User);

            var extension = Path.GetExtension(avatar.FileName);

            var blobName = $"{user.Id}{extension}";
            var connectionString = "";
            var blobServiceClient = new BlobServiceClient(connectionString);
            string containerName = "useravatar";
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = avatar.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var avatarUrl = blobClient.Uri.ToString();
            user.AboutMe = updatedUser.AboutMe;
            user.AvatarUrl = avatarUrl;

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