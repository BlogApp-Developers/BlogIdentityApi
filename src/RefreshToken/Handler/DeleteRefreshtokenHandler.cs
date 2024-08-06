namespace BlogIdentityApi.RefreshToken.Handler;

using BlogIdentityApi.RefreshToken.Repositories;
using BlogIdentityApi.RefreshToken.Entity;
using BlogIdentityApi.RefreshToken.Command;
using MediatR;

public class DeleteRefreshtokenHandler : IRequestHandler<DeleteRefreshTokenCommand>
{
    private readonly IRefreshTokenRepository repository;
    public DeleteRefreshtokenHandler(IRefreshTokenRepository repository)
    {
        this.repository = repository;
    }

    public async Task Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if(request.UserId.GetType() != typeof(Guid))
        {
            throw new ArgumentException("userId is not valid for token");
        }
        
        var refreshToken = new RefreshToken(){
            Token = request.Token,
            UserId = request.UserId
        };
        await repository.DeleteAsync(refreshToken);
    }
}
