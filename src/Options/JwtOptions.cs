using System.Text;

namespace BlogIdentityApi.Options;

public class JwtOptions
{
    public required string Key { get; set; }
    public byte[] KeyInBytes => Encoding.ASCII.GetBytes(Key);
    public required int LifeTimeInMinutes { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}
