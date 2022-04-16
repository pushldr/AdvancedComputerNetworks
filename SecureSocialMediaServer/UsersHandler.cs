using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer;

public static class UsersHandler
{
    // This is a terrible way to store this data but for the purpose of this assignment it's fine as we are
    // concerned specifically with crypto section of it
    private static readonly Dictionary<string, string> UsernamePasswords = new ();
    private static readonly Dictionary<string, string> UsernamePKs = new ();

    public static bool RegisterUser(string username, string password, string publickey)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        if (string.IsNullOrEmpty(password))
            return false;

        if (string.IsNullOrEmpty(publickey))
            return false;
        
        if (UsernamePasswords.ContainsKey(username))
            return false;

        UsernamePasswords[username] = password;
        UsernamePKs[username] = publickey;

        if (UsernamePasswords.Count == 1)
            GroupHandler.Init(username);
        

        return true;
    }

    public static bool IsValid(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        if (string.IsNullOrEmpty(password))
            return false;
        
        if (UsernamePasswords.TryGetValue(username, out var pass))
            if (password.Equals(pass))
                return true;

        return false;
    }

    public static IEnumerable<GetPublicKeysResponse> GetPublicKeysResponse(HashSet<string> users)
    {
        List<GetPublicKeysResponse> publicKeysResponses = new List<GetPublicKeysResponse>();
        foreach (var user in users)
        {
            if(UsernamePKs.ContainsKey(user))
                publicKeysResponses.Add(new GetPublicKeysResponse(user, UsernamePKs[user]));
        }
        return publicKeysResponses;
    }
}