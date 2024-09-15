namespace BlogIdentityApi.Controllers;

using BlogIdentityApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using Azure.Storage.Blobs;
using BlogIdentityApi.Dtos.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BlogIdentityApi.RefreshToken.Command;
using BlogIdentityApi.Options;
using MediatR;
using Microsoft.Extensions.Options;

[Route("api/[controller]/[action]")]
public class SettingsController : ControllerBase
{
    private readonly BlogIdentityDbContext dbContext;
    private readonly UserManager<User> userManager;
    private readonly IUserRepository userRepository;
    private readonly JwtOptions jwtOptions;
    private readonly ISender sender;

    public SettingsController(BlogIdentityDbContext dbContext, UserManager<User> userManager, IUserRepository userRepository, ISender sender, IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.userRepository = userRepository;
        this.sender = sender;
        this.jwtOptions = jwtOptionsSnapshot.Value;
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
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=blogteamstorage;AccountKey=Jj3XzYJg5sReLiUkj+1X6eap4T8DHLyY3uwbR9OwsqAx+q+HSHgdvKz0EPRWKgUOYChVJ6GCDFiQ+AStSO+mpg==;EndpointSuffix=core.windows.net";
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

            var roles = await userManager.GetRolesAsync(user);

            var claims = roles
                .Select(roleStr => new Claim(ClaimTypes.Role, roleStr))
                .Append(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()))
                .Append(new Claim(ClaimTypes.Email, user.Email ?? "not set"))
                .Append(new Claim(ClaimTypes.Name, user.UserName ?? "not set"))
                .Append(new Claim(ClaimTypes.UserData, user.AvatarUrl ?? "not set"));

            var signingKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtOptions.LifeTimeInMinutes),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var tokenStr = handler.WriteToken(jwtToken);


            var createRefreshTokenCommand = new CreateRefreshTokenCommand {
                UserId = user.Id,
                Token = Guid.NewGuid(),
            };

            await sender.Send(createRefreshTokenCommand);

            var redirectUrl = $"http://localhost:5234/HandleLoginTokens?access={tokenStr}&refresh={createRefreshTokenCommand.Token.ToString("N")}&userId={user.Id}";
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            return base.BadRequest(ex.Message);
        }
    }
}