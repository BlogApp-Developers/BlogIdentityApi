namespace BlogIdentityApi.User.Commands;

using MediatR;

public class ChangeAboutMeUserCommand : IRequest
{
    public int UserId { get; set; }
    public string? AboutMe{set; get;}
}
