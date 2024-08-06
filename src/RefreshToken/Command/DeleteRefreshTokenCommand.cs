namespace BlogIdentityApi.RefreshToken.Command;

using MediatR;

public class DeleteRefreshTokenCommand : IRequest
{
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
}
