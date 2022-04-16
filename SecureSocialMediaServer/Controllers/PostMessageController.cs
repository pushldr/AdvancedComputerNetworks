using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PostMessageController : ControllerBase
{
    
    private readonly ILogger<PostMessageController> _logger;

    public PostMessageController(ILogger<PostMessageController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "PostMessage")]
    public StatusCodeResult Post([FromBody]PostMessageRequest value)
    {
        if (!UsersHandler.IsValid(value.username, value.password))
            return Unauthorized();

        if (string.IsNullOrEmpty(value.message))
            return BadRequest();

        if (!GroupHandler.Has(value.username))
            return Unauthorized();
            
        GroupHandler.Messages.Add((value.username,value.message));

        return Ok();
    }
}