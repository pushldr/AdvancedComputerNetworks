using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicKeysController : ControllerBase
{
    
    private readonly ILogger<PublicKeysController> _logger;

    public PublicKeysController(ILogger< PublicKeysController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "GetPublicKeys")]
    public IEnumerable<GetPublicKeysResponse> Post([FromBody]GetPublicKeysRequest value)
    {
        return UsersHandler.GetPublicKeysResponse(GroupHandler.GetUsers());
    }
}