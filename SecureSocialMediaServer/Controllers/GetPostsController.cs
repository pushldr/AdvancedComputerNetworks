using Microsoft.AspNetCore.Mvc;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GetPostsController : ControllerBase
{
    
    private readonly ILogger<GetPostsController> _logger;

    public GetPostsController(ILogger<GetPostsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetPosts")]
    public IEnumerable<PostReponse> Get()
    {
        var list = new List<PostReponse>();
        foreach (var post in GroupHandler.Messages)
        {
            list.Add(new PostReponse()
            {
                username = post.Item1, message = post.Item2
            });
        }
        return list;
    }
}