using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class IsAdminController : ControllerBase
{

    private readonly ILogger<IsAdminController> _logger;

    public IsAdminController(ILogger<IsAdminController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "IsAdmin")]
    public string Post([FromBody] IsAdminRequest value)
    {
        return GroupHandler.IsAdmin(value.username).ToString();
    }
}