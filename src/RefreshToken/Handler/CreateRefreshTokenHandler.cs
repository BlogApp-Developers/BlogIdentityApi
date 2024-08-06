namespace BlogIdentityApi.RefreshToken.Handler;

using System.Threading;
using System.Threading.Tasks;
using BlogIdentityApi.RefreshToken.Entity;
using BlogIdentityApi.RefreshToken.Repositories;
using BlogIdentityApi.RefreshToken.Command;
using MediatR;

public class CreateRefreshTokenHandler : IRequestHandler<CreateRefreshTokenCommand>
{
    private readonly IRefreshTokenRepository repository;
    public CreateRefreshTokenHandler(IRefreshTokenRepository repository)
    {
        this.repository = repository;
    }

    public async Task Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if(request.UserId.GetType() != typeof(Guid))
        {
            throw new ArgumentException("userId is not valid for token");
        }
        
        var refreshToken = new RefreshToken(){
            Token = request.Token,
            UserId = request.UserId
        };
        await repository.CreateAsync(refreshToken);
    }
}
