namespace BlogIdentityApi.RefreshToken.Command;

using MediatR;

public class CreateRefreshTokenCommand : IRequest
{
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
}
