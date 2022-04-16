using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RegistrationController : ControllerBase
{
    
    private readonly ILogger< RegistrationController> _logger;

    public RegistrationController(ILogger< RegistrationController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "Register")]
    public StatusCodeResult Post([FromBody]UserRegistration value)
    {
        if (UsersHandler.RegisterUser(value.username, value.password, value.publickey))
            return StatusCode(200);
        return StatusCode(403);
    }
}