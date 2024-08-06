namespace BlogIdentityApi.Verification.Base;

public interface IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string message);
}
