using BlogIdentityApi.Dtos.Models;
using BlogIdentityApi.Verification.Base;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.IdentityModel.Tokens.Jwt;
using BlogIdentityApi.RefreshToken.Command;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using BlogIdentityApi.Options;
using System.ComponentModel.DataAnnotations;
using BlogIdentityApi.RefreshToken.Query;
using Microsoft.AspNetCore.Authorization;

namespace BlogIdentityApi.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class IdentityController : Controller
{
    private readonly SignInManager<User.Models.User> signInManager;
    private readonly UserManager<User.Models.User> userManager;
    private readonly IDataProtector dataProtector;
    private readonly IValidator<RegistrationDto> userValidator;
    private readonly IValidator<LoginDto> userLoginValidator;
    private readonly IEmailService emailService;
    private readonly ISender sender;
    private readonly JwtOptions jwtOptions;

    public IdentityController(ISender sender, 
        IValidator<LoginDto> userLoginValidator, 
        SignInManager<User.Models.User> signInManager, 
        UserManager<User.Models.User> userManager, 
        IValidator<RegistrationDto> userValidator, 
        IDataProtectionProvider dataProtectionProvider, 
        IEmailService emailService,
        IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot)
    {
        this.sender = sender;
        this.userLoginValidator = userLoginValidator;
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.dataProtector = dataProtectionProvider.CreateProtector("identity");
        this.userValidator = userValidator;
        this.emailService = emailService;
        this.jwtOptions =jwtOptionsSnapshot.Value;
    }

    [HttpPost]
    [ActionName("Login")]
    public async Task<IActionResult> SignIn([FromForm] LoginDto loginDto)
    {
        try
        {
            var validationResult = userLoginValidator.Validate(loginDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(loginDto.Email!);

            if (user == null)
            {
                return BadRequest("Incorrect email!");
            }

            var tokenData = $"{loginDto.Email}:{loginDto.Name}";
            var token = dataProtector.Protect(tokenData);
            var confirmationLink = Url.Action("ConfirmLogin", "Identity", new { token }, Request.Scheme);
            var message = $"Please confirm your login by clicking on the link: {HtmlEncoder.Default.Encode(confirmationLink!)}";

            await emailService.SendEmailAsync(loginDto.Email!, "Confirm your login", message);
            TempData["Email"] = loginDto.Email;
            
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [ActionName("ConfirmLogin")]
    public async Task<IActionResult> ConfirmEmailLogin(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid token");
        }

        var tokenData = dataProtector.Unprotect(token);
        var dataParts = tokenData.Split(':');
        if (dataParts.Length != 2)
        {
            return BadRequest("Invalid token data");
        }

        var email = dataParts[0];
        var name = dataParts[1];

        var foundUser = await userManager.FindByEmailAsync(email);
        if (foundUser is null)
        {
            return BadRequest("User not found");
        }

        await signInManager.SignInAsync(foundUser, isPersistent: true);

        var roles = await userManager.GetRolesAsync(foundUser);

        var claims = roles
            .Select(roleStr => new Claim(ClaimTypes.Role, roleStr))
            .Append(new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()))
            .Append(new Claim(ClaimTypes.Email, foundUser.Email ?? "not set"))
            .Append(new Claim(ClaimTypes.Name, foundUser.UserName ?? "not set"));

        var signingKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtOptions.LifeTimeInMinutes),
            signingCredentials: signingCredentials
        );

        var handler = new JwtSecurityTokenHandler();
        var tokenStr = handler.WriteToken(jwtToken);


        var createRefreshTokenCommand = new CreateRefreshTokenCommand {
            UserId = foundUser.Id,
            Token = Guid.NewGuid(),
        };

        await sender.Send(createRefreshTokenCommand);

        return Ok(new {
            refresh = createRefreshTokenCommand.Token,
            access = tokenStr,
        });
    }

    [HttpPost]
    [ActionName("Registration")]
    public async Task<IActionResult> SignUp([FromForm] RegistrationDto registrationDto)
    {
        try
        {
            var validationResult = userValidator.Validate(registrationDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            var tokenData = $"{registrationDto.Email}:{registrationDto.Name}";
            var token = dataProtector.Protect(tokenData);
            var confirmationLink = Url.Action("ConfirmRegistration", "Identity", new { token }, Request.Scheme);
            var message = $"Please confirm your registration by clicking on the link: {HtmlEncoder.Default.Encode(confirmationLink!)}";

            await emailService.SendEmailAsync(registrationDto.Email!, "Confirm your email", message);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [ActionName("ConfirmRegistration")]
    public async Task<IActionResult> ConfirmEmailRegistration(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid token");
        }

        var tokenData = dataProtector.Unprotect(token);
        var dataParts = tokenData.Split(':');
        if (dataParts.Length != 2)
        {
            return BadRequest("Invalid token data");
        }

        var email = dataParts[0];
        var name = dataParts[1];

        var user = new User.Models.User
        {
            Email = email,
            UserName = name,
            AvatarUrl = "Assets/UserAvatar/DefaultAvatar.png"
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join("\n", result.Errors.Select(error => error.Description)));
        }

        var foundUser = await userManager.FindByEmailAsync(user.Email);

        await signInManager.SignInAsync(foundUser!, isPersistent: true);

        var roles = await userManager.GetRolesAsync(foundUser!);

        var claims = roles
            .Select(roleStr => new Claim(ClaimTypes.Role, roleStr))
            .Append(new Claim(ClaimTypes.NameIdentifier, foundUser!.Id.ToString()))
            .Append(new Claim(ClaimTypes.Email, foundUser.Email ?? "not set"))
            .Append(new Claim(ClaimTypes.Name, foundUser.UserName ?? "not set"));

        var signingKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtOptions.LifeTimeInMinutes),
            signingCredentials: signingCredentials
        );

        var handler = new JwtSecurityTokenHandler();
        var tokenStr = handler.WriteToken(jwtToken);

        var createRefreshTokenCommand = new CreateRefreshTokenCommand {
            UserId = foundUser.Id,
            Token = Guid.NewGuid(),
        };

        await sender.Send(createRefreshTokenCommand);

        HttpContext.Response.Headers["access"] = tokenStr;
        HttpContext.Response.Headers["refresh"] = createRefreshTokenCommand.Token.ToString("N");
        
        return Redirect($"http://localhost:5234/choosetags?userId={foundUser!.Id}");
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateToken([Required]Guid refresh) 
    {
        try
        {
            var tokenStr = base.HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if(tokenStr is null) {
                return base.StatusCode(401);
            }

            if(tokenStr.StartsWith("Bearer ")) {
                tokenStr = tokenStr.Substring("Bearer ".Length);
            }

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationResult = await handler.ValidateTokenAsync(
                tokenStr,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes)
                }
            );

            if(tokenValidationResult.IsValid == false) {
                return BadRequest(tokenValidationResult.Exception);
            }

            var token = handler.ReadJwtToken(tokenStr);

            Claim? idClaim = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if(idClaim is null) {
                return BadRequest($"Token has no claim with type '{ClaimTypes.NameIdentifier}'");
            }

            Guid.TryParse(idClaim.Value, out Guid userId);
            var userIdstr = idClaim.Value;

            var foundUser = await userManager.FindByIdAsync(userIdstr);

            if(foundUser is null) {
                return BadRequest($"User not found by id: '{userId}'");
            }

            var getRefrshCTokenQuery = new GetRefreshTokenQuery()
            {
                Token = refresh,
                UserId = userId,
            };

            var oldRefreshToken = await sender.Send(getRefrshCTokenQuery);

            if(oldRefreshToken is null)
            {
                var deleteRangeRefreshTokenCommand = new DeleteRangeRefreshTokenCommand()
                {
                    UserId = userId
                };
                await sender.Send(deleteRangeRefreshTokenCommand);
                return BadRequest("Refresh token not found!");
            }


            var deleteRefreshTokenCommand = new DeleteRefreshTokenCommand()
            {
                Token = oldRefreshToken.Token,
                UserId = userId,
            };

            await sender.Send(deleteRefreshTokenCommand);

            var createRefreshTokenCommand = new CreateRefreshTokenCommand {
                UserId = foundUser.Id,
                Token = Guid.NewGuid(),
            };

            await sender.Send(createRefreshTokenCommand);

            var roles = await userManager.GetRolesAsync(foundUser);

            var claims = roles
                .Select(roleStr => new Claim(ClaimTypes.Role, roleStr))
                .Append(new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()))
                .Append(new Claim(ClaimTypes.Email, foundUser.Email ?? "not set"))
                .Append(new Claim(ClaimTypes.Name, foundUser.UserName ?? "not set"));

            var signingKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var newToken = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtOptions.LifeTimeInMinutes),
                signingCredentials: signingCredentials
            );

            var newTokenStr = handler.WriteToken(newToken);

            return Ok(new {
                refresh = createRefreshTokenCommand.Token,
                access = newTokenStr,
            });
        }
        catch(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> LogOut([Required]Guid refresh) 
    {
        try
        {
            var tokenStr = base.HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if(tokenStr is null) {
                return base.StatusCode(401);
            }

            if(tokenStr.StartsWith("Bearer ")) {
                tokenStr = tokenStr.Substring("Bearer ".Length);
            }

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationResult = await handler.ValidateTokenAsync(
                tokenStr,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes)
                }
            );

            if(tokenValidationResult.IsValid == false) {
                return BadRequest(tokenValidationResult.Exception);
            }

            var token = handler.ReadJwtToken(tokenStr);

            Claim? idClaim = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if(idClaim is null) {
                return BadRequest($"Token has no claim with type '{ClaimTypes.NameIdentifier}'");
            }

            Guid.TryParse(idClaim.Value, out Guid userId);
            var userIdstr = idClaim.Value;

            var foundUser = await userManager.FindByIdAsync(userIdstr);

            if(foundUser is null) {
                return BadRequest($"User not found by id: '{userId}'");
            }

            var deleteRefreshTokenCommand = new DeleteRefreshTokenCommand()
            {
                Token = refresh,
                UserId = userId,
            };

            await sender.Send(deleteRefreshTokenCommand);

            return Ok();
        }
        catch(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}




