using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RemoveUserController : ControllerBase
{
    
    private readonly ILogger<RemoveUserController> _logger;

    public RemoveUserController(ILogger<RemoveUserController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "RemoveUser")]
    public StatusCodeResult Post([FromBody]UserControlRequest value)
    {
        if (!UsersHandler.IsValid(value.username, value.password))
            return Unauthorized();
        
        if (!GroupHandler.IsAdmin(value.username))
            return Unauthorized();

        if (string.IsNullOrEmpty(value.user))
            return NotFound();
        
        GroupHandler.Remove(value.user);

        return Ok();
    }
}