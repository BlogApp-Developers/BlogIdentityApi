namespace BlogIdentityApi.RefreshToken.Handler;

using BlogIdentityApi.RefreshToken.Repositories;
using BlogIdentityApi.RefreshToken.Entity;
using BlogIdentityApi.RefreshToken.Query;
using MediatR;

public class GetRefreshTokenHandler : IRequestHandler<GetRefreshTokenQuery, RefreshToken?>
{
    private readonly IRefreshTokenRepository repository;
    public GetRefreshTokenHandler(IRefreshTokenRepository repository)
    {
        this.repository = repository;
    }

    public async Task<RefreshToken?> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        if(request.UserId.GetType() != typeof(Guid))
        {
            throw new ArgumentException("userId is not valid for token");
        }
        
        var refreshToken = new RefreshToken(){
            Token = request.Token,
            UserId = request.UserId
        };
        return await repository.GetRefreshTokenAsync(refreshToken);
    }
}