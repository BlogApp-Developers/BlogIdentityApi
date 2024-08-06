namespace BlogIdentityApi.RefreshToken.Handler;

using BlogIdentityApi.RefreshToken.Repositories;
using BlogIdentityApi.RefreshToken.Entity;
using BlogIdentityApi.RefreshToken.Command;
using MediatR;

public class DeleteRangeRefreshTokenHandler: IRequestHandler<DeleteRangeRefreshTokenCommand>
{
    private readonly IRefreshTokenRepository repository;
    public DeleteRangeRefreshTokenHandler(IRefreshTokenRepository repository)
    {
        this.repository = repository;
    }

    public async Task Handle(DeleteRangeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if(request.UserId.GetType() != typeof(Guid))
        {
            throw new ArgumentException("userId is not valid for token");
        }
        
        var refreshToken = new RefreshToken(){
            UserId = request.UserId
        };
        await repository.DeleteRangeRefreshTokensAsync(refreshToken);
    }
}
