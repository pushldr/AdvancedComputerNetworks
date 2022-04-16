using System.Runtime.CompilerServices;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaServer;

public static class GroupHandler
{
    public static string admin;
    public static List<(string, string)> Messages = new ();
    private static HashSet<string> Users = new ();
    private static Dictionary<string, string> newkeys = new ();
    public static void Init(string a)
    {
        admin = a;
        Messages.Clear();
        Users.Clear();
    }

    public static void Add(string user) => Users.Add(user);
    
    public static void Remove(string user) => Users.Remove(user);
    

    public static bool Has(string user) => Users.Contains(user) || admin.Equals(user);

    public static bool HasNewKey(string user, out string newKey)
    {
        if (newkeys.TryGetValue(user, out newKey))
        {
            newkeys.Remove(user);
            return true;
        }

        return false;
    }

    public static void AddNewKey(string user, string key)
    {
        newkeys.Remove(user);
        newkeys.Add(user, key);
    }

    public static bool IsAdmin(string user)
    {
        return admin.Equals(user);
    }

    public static HashSet<string> GetUsers() => Users;

}