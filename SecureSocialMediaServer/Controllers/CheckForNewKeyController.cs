using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckForNewKeyController : ControllerBase
{

    private readonly ILogger<CheckForNewKeyController> _logger;

    public CheckForNewKeyController(ILogger<CheckForNewKeyController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "CheckForNewKey")]
    public string Post([FromBody]CheckForNewKeyRequest value)
    {
        if (!UsersHandler.IsValid(value.username, value.password))
            return "NO";

        if (GroupHandler.HasNewKey(value.username, out var newKey))
            return newKey;

        return "NO";
    }
}