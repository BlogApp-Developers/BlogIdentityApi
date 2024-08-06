namespace BlogIdentityApi.RefreshToken.Query;

using MediatR;
using BlogIdentityApi.RefreshToken.Entity;

public class GetRefreshTokenQuery : IRequest<RefreshToken>
{
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
}
