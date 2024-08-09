using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
public class FollowController : ControllerBase
{
    [Authorize]
    public async Task<IActionResult> Follow()
    {
        

        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> Unfollow()
    {
        return Ok();
    }
}