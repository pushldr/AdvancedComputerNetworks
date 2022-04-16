using System.Security.Cryptography;
using System.Text;
using SecureSocialMediaServer.Models;

namespace SecureSocialMediaApp;

internal static class Program
{
    public static ApiClient ApiClient = new ("https://localhost:7294/");
    private static string username;
    private static string password;
    private static bool isAdmin;
    private static RSACryptoServiceProvider rsa;

    static async Task Main(string[] args)
    {
        rsa = new RSACryptoServiceProvider(2048);
        Console.WriteLine("Please enter a username: ");
        username = Console.ReadLine();
        Console.WriteLine("Please enter a password: ");
        password = Console.ReadLine();

        if (await ApiClient.Register(username, password, Convert.ToBase64String(rsa.ExportRSAPublicKey())))
        {
            Console.WriteLine("Registered successfully");
        }
        else
        {
            Console.WriteLine("Registration failed");
            Environment.Exit(1);
        }

        isAdmin = await ApiClient.IsAdmin(username);
        if (isAdmin)
        {
            GenerateKey();
            Console.WriteLine("You are the admin");
        }

        PrintUsage();
        await InputThread();
        
    }

    private static void PrintUsage()
    {
        Console.WriteLine();
        Console.WriteLine("============== USAGE ==============");
        Console.WriteLine("[?] q              -> Quits program");
        Console.WriteLine("[?] cls            -> Clears console");
        Console.WriteLine("[?] ?              -> Prints usage");
        Console.WriteLine("[?] add [user]     -> Adds user to group");
        Console.WriteLine("[?] remove [user]  -> Removes user from group");
        Console.WriteLine("[?] post [message] -> Posts encrypted message");
        Console.WriteLine("[?] getposts       -> Gets list of posts");
        Console.WriteLine();
    }

    private static async Task InputThread()
    {
        while (true)
        {
            var input = Console.ReadLine() ?? "";
            if (input.Equals("q"))
            {
                Console.WriteLine("[!] Shutting down...");
                Environment.Exit(1);
            }
            else if (input.Equals("cls"))
                Console.Clear();
            else if (input.Equals("?"))
                PrintUsage();
            else if (input.StartsWith("add "))
            {
                if (await ApiClient.AddUser(username, password, input.Split(' ')[1]))
                {
                    Console.WriteLine("Added user successfully");
                    var publicKeys = await ApiClient.GetPublicKeys(username);
                    await UpdateKeys(publicKeys);
                }
                else
                    Console.WriteLine("Failed to add user");
            }
            else if (input.StartsWith("remove "))
            {
                if (await ApiClient.RemoveUser(username, password, input.Split(' ')[1]))
                {
                    Console.WriteLine("Removed user successfully");
                    var publicKeys = await ApiClient.GetPublicKeys(username);
                    await UpdateKeys(publicKeys);
                }
                else
                    Console.WriteLine("Failed to remove user");
            }
            else if (input.StartsWith("post "))
            {
                await CheckForNewKey();

                var toEncrypt = input.Remove(0, 5);
                var encrypted = Encrypt(toEncrypt, key);
                
                if (await ApiClient.PostMessage(username, password, encrypted))
                {
                    Console.WriteLine("Successfully posted message");
                }
                else
                {
                    Console.WriteLine("Failed to post message");
                }
            }
            else if (input.Equals("getposts"))
            {
                await CheckForNewKey();

                var posts = await ApiClient.GetPosts();

                Console.WriteLine("======== Posts ========");
                foreach (var post in posts)
                {
                    Console.WriteLine($"[*] User:    {post.username}");

                    try
                    {
                        Console.WriteLine($"    Message: {Decrypt(post.message, key)}");
                    }
                    catch
                    {
                        Console.WriteLine($"    Message: {post.message}");
                    }
                }
                Console.WriteLine("=======================");
            }
            // else if (input.StartsWith())

        }
    }

    private static async Task CheckForNewKey()
    {
        var response = await ApiClient.CheckForNewKey(username, password);
        if (response.Item1)
        {
            key = rsa.Decrypt(Convert.FromBase64String(response.Item2), true);
        }
    }

    private static string Encrypt(string message, byte[] key)
    {
        // Create the streams used for encryption.  
        var data = Encoding.Default.GetBytes(message);
        using var aes = Aes.Create();
        using ICryptoTransform encryptor = aes.CreateEncryptor(key, new byte[16]);
        var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
        return Convert.ToBase64String(encrypted);
    }
    private static string Decrypt(string message, byte[] key)
    {
        // Create the streams used for decryption.  
        var cipher = Convert.FromBase64String(message);
        using var aes = Aes.Create();
        using ICryptoTransform decryptor = aes.CreateDecryptor(key, new byte[16]);
        var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.Default.GetString(decrypted);
    }

    private static async Task UpdateKeys(IEnumerable<GetPublicKeysResponse> publicKeysResponses)
    {
        GenerateKey();
        var updateRequest = new UpdatePublicKeysRequest();
        updateRequest.username = username;
        updateRequest.password = password;
        var keys = new List<UpdatePublicKeysSegment>();
        foreach (var publicKeysResponse in publicKeysResponses)
        {
            using var RSA = new RSACryptoServiceProvider();
            RSA.ImportRSAPublicKey(Convert.FromBase64String(publicKeysResponse.publickey), out int _);
            var encrypted = RSA.Encrypt(key, true);
            keys.Add(new UpdatePublicKeysSegment(publicKeysResponse.username, Convert.ToBase64String(encrypted)));
        }

        updateRequest.keys = keys;
        if (await ApiClient.UpdateKeys(updateRequest))
        {
            Console.WriteLine("Successfully updated keys");
        }
        else
        {
            Console.WriteLine("Failed to update keys");
            Environment.Exit(0);
        }
    }

    private static void GenerateKey()
    {
        var aes = Aes.Create();
        aes.GenerateKey();
        key = aes.Key;
        Console.WriteLine($"[!] New key is {Convert.ToBase64String(key)}");
    }

    private static byte[] key;

}