namespace SecureSocialMediaServer.Models;

public class UpdatePublicKeysSegment
{
    public string username{get; set;}
    public string publickey{get; set;}

    public UpdatePublicKeysSegment(string username, string publickey)
    {
        this.username = username;
        this.publickey = publickey;
    }
}