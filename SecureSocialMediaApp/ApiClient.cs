using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaApp;

public class ApiClient
{
    private string baseUrl;
    private HttpClientHandler _handler;
    public ApiClient(string baseUrl)
    {
        this.baseUrl = baseUrl;
       _handler = new HttpClientHandler();
       _handler.ClientCertificateOptions = ClientCertificateOption.Manual;
       _handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
    }

    public async Task<HttpResponseMessage> SendPost(string endpoint, object body)
    {
        var url = $"{baseUrl}{endpoint}";

        var client = new HttpClient(_handler);
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result =  await client.PostAsync(url, content);
        return result;
    }
    
    public async Task<HttpResponseMessage> SendGet(string endpoint)
    {
        var url = $"{baseUrl}{endpoint}";

        var client = new HttpClient(_handler);
        var result =  await client.GetAsync(url);
        return result;
    }

    public async Task<bool> Register(string username, string password, string publicKey)
    { 
        var response = await SendPost("Registration", new UserRegistration() {username = username, password = password, publickey = publicKey});
        if (response.StatusCode == HttpStatusCode.OK)
            return true;
        return false;
    }
    
    public async Task<bool> AddUser(string username, string password, string user)
    { 
        var response = await SendPost("AddUser", new UserControlRequest() {username = username, password = password, user = user});
        if (response.StatusCode == HttpStatusCode.OK)
            return true;
        return false;
    }
    public async Task<bool> RemoveUser(string username, string password, string user)
    { 
        var response = await SendPost("RemoveUser", new UserControlRequest() {username = username, password = password, user = user});
        if (response.StatusCode == HttpStatusCode.OK)
            return true;
        return false;
    }
    
    public async Task<bool> IsAdmin(string username)
    {
        var response = await SendPost("IsAdmin", new IsAdminRequest(){username = username});
        var result = await response.Content.ReadAsStringAsync();
        if ("True".Equals(result))
            return true;
        return false;
    }
    
    public async Task<IEnumerable<GetPublicKeysResponse>> GetPublicKeys(string excluded)
    {
        var response = await SendPost("PublicKeys", new GetPublicKeysRequest() {exclude = excluded});
        var result = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.OK)
            return JsonSerializer.Deserialize<IEnumerable<GetPublicKeysResponse>>(result) ?? new List<GetPublicKeysResponse>();
        Console.WriteLine("[!] Failed to fetch public keys");
        return  new List<GetPublicKeysResponse>();
    }
    
    
    public async Task<bool> UpdateKeys(UpdatePublicKeysRequest updatePublicKeysRequest)
    { 
        var response = await SendPost("UpdateKeys", updatePublicKeysRequest);
        if (response.StatusCode == HttpStatusCode.OK)
            return true;
        return false;
    }
    
    public async Task<(bool, string)> CheckForNewKey(string username, string password)
    {
        var response = await SendPost("CheckForNewKey", new CheckForNewKeyRequest() { username = username, password = password});
        var result = await response.Content.ReadAsStringAsync();
        if ("NO".Equals(result))
            return (false, "");
        return (true, result);
    }
    
    public async Task<bool> PostMessage(string username, string password, string message)
    { 
        var response = await SendPost("PostMessage", new PostMessageRequest() {username = username, password = password, message = message});
        if (response.StatusCode == HttpStatusCode.OK)
            return true;
        return false;
    }
    
    public async Task<IEnumerable<PostReponse>> GetPosts()
    { 
        var response = await SendGet("GetPosts");
        var result = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.OK)
            return JsonSerializer.Deserialize<IEnumerable<PostReponse>>(result) ?? new List<PostReponse>();
        Console.WriteLine("[!] Failed to fetch messages");
        return new List<PostReponse>();
    }

    
}