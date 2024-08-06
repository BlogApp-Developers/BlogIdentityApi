namespace BlogIdentityApi.RefreshToken.Command;

using MediatR;

public class DeleteRangeRefreshTokenCommand : IRequest
{
    public Guid UserId { get; set; }
}
