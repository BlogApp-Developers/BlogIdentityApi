namespace BlogIdentityApi.Base.Methods;

public interface IIsFollowing
{
    public bool IsFollowing(Guid personalId, Guid id);
}