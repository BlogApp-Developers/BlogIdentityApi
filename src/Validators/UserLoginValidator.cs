using BlogIdentityApi.Dtos.Models;
using FluentValidation;

namespace BlogIdentityApi.Validators;

public class UserLoginValidator: AbstractValidator<LoginDto>
{
     public UserLoginValidator()
    {
        base.RuleFor(u => u.Email)
                        .NotEmpty()
                        .EmailAddress();
    }

        
}

