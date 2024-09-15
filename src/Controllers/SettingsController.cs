namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using Azure.Storage.Blobs;

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
    public async Task<IActionResult> EditProfile(User updatedUser, IFormFile avatar)
    {
        try
        {
            var extension = Path.GetExtension(avatar.FileName);

            var blobName = $"{updatedUser.Id}{extension}";
            var connectionString = "https://blogteamstorage.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(connectionString);
            string containerName = "useravatar";
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = avatar.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var avatarUrl = blobClient.Uri.ToString();
            updatedUser.AvatarUrl = avatarUrl;

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