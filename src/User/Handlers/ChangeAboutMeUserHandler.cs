namespace BlogIdentityApi.User.Handlers;

using System.Threading;
using System.Threading.Tasks;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class ChangeAboutMeUserHandler : IRequestHandler<ChangeAboutMeUserCommand>
{
    public UserManager<User> userManager { get; set; }

    public ChangeAboutMeUserHandler(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }
    public async Task Handle(ChangeAboutMeUserCommand request, CancellationToken cancellationToken)
    {
        if(request.UserId <= 0)
        {
            throw new ArgumentException("userId cannot be <= 0");
        }
        else if(string.IsNullOrEmpty(request.AboutMe) || string.IsNullOrWhiteSpace(request.AboutMe))
        {
            throw new ArgumentException("AboutMe is empty");
        }

        var foundUser = await userManager.FindByIdAsync(request.UserId.ToString());

        if(foundUser is null)
        {
            throw new ArgumentException($"cannot find user with id = {request.UserId}");
        }
        else if(foundUser.AboutMe is not null && foundUser.AboutMe.Trim() == request.AboutMe.Trim())
        {
            throw new ArgumentException($"new AboutMe is similar to old");
        }

        foundUser.AboutMe = request.AboutMe;

        await userManager.UpdateAsync(foundUser);
    }
}
