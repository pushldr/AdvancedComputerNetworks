using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class UpdateKeysController : ControllerBase
{
    
    private readonly ILogger<UpdateKeysController> _logger;

    public UpdateKeysController(ILogger<UpdateKeysController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "UpdateKeys")]
    public StatusCodeResult Post([FromBody]UpdatePublicKeysRequest value)
    {
        if (!UsersHandler.IsValid(value.username, value.password))
            return Unauthorized();

        if (!GroupHandler.IsAdmin(value.username))
            return Unauthorized();


        foreach (var newEntry in value.keys)
        {
            GroupHandler.AddNewKey(newEntry.username, newEntry.publickey);
        }
        
        GroupHandler.Messages.Clear();

        return Ok();
    }
}