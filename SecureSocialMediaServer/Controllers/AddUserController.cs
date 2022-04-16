using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AddUserController : ControllerBase
{
    
    private readonly ILogger<AddUserController> _logger;

    public AddUserController(ILogger<AddUserController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "AddUser")]
    public StatusCodeResult Post([FromBody]UserControlRequest value)
    {
        if (!UsersHandler.IsValid(value.username, value.password))
            return Unauthorized();
        
        if (!GroupHandler.IsAdmin(value.username))
            return Unauthorized();

        if (string.IsNullOrEmpty(value.user))
            return NotFound();
        
        Console.WriteLine($"[*] Adding {value.user} to group");
        
        GroupHandler.Add(value.user);

        return Ok();
    }
}