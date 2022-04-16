namespace SecureSocialMediaServer.Models;

public class UpdatePublicKeysRequest
{
    public string username{get; set;}
    public string password{get; set;}
    public IEnumerable<UpdatePublicKeysSegment> keys{get; set;}
}