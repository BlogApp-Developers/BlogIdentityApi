using BlogIdentityApi.Dtos;
using BlogIdentityApi.Dtos.Models;
using FluentValidation;

namespace BlogIdentityApi.Validators;

public class UserRegistrationValidator : AbstractValidator<RegistrationDto>
{
     public UserRegistrationValidator()
    {


        base.RuleFor(u => u.Email)
                        .NotEmpty()
                        .EmailAddress();

        


        base.RuleFor(u => u.Name)
            .NotEmpty();

        


    }

        
}
