using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebIntegrationScript : MonoBehaviour
{

    private static float timeSinceLastConnection;
    private static readonly float pingInterval = 60f;
    private static bool connectionRunning = false, loopRunning = false;
    private static string serverPublicKey, clientPrivateKey, clientPublicKey;
    private static readonly string baseURL = "https://odrestserver.onrender.com/";
    private static WebRequest request = WebRequest.GetKey;
    private static Dictionary<Endpoint, string> endpoints = new Dictionary<Endpoint, string>
        {

            { Endpoint.PublicKey , "UserListing/publickey" },
            { Endpoint.AddUser , "UserListing/add" },
            { Endpoint.Login , "UserListing/login" },
            { Endpoint.ClearUsers , "UserListing/clear" },
            { Endpoint.TestReadUsers , "UserListing/testreader" },
            { Endpoint.ClearScoresAndAchievements , "AchievementListing/clear" },
            { Endpoint.AddAchievement , "AchievementListing/addachievement" },
            { Endpoint.OwnAchievements , "AchievementListing/getownachievements" },
            { Endpoint.AchievementsEarned, "AchievementListing/achievementsearned" },
            { Endpoint.AddHighscore , "AchievementListing/addscore" },
            { Endpoint.OwnHighscore , "AchievementListing/getownscore" },
            { Endpoint.Leaderboard , "AchievementListing/getleaderboard" },
            { Endpoint.Ping , "UserListing/ping" }

        };


    public static WebRequest Request
    {

        get => request;
        set
        {

            if (value == WebRequest.None && value != request)
            {

                timeSinceLastConnection = Time.unscaledTime;
                connectionRunning = true;

            }

            if (value != request)
                request = value;

        }

    }


    private static string ServerPublicKey
    {

        get => serverPublicKey;
        set
        {

            if (request == WebRequest.GetKey)
                Request = WebRequest.None;

            serverPublicKey = value;

        }

    }

    private async void Awake()
    {

        timeSinceLastConnection = Time.unscaledTime;

        using (RSA rsa = RSA.Create())
        {

            clientPrivateKey = rsa.ToXmlString(true);
            clientPublicKey = rsa.ToXmlString(false);

        }

        if (!loopRunning)
            await CheckTasks();

    }


    private async Task CheckTasks()
    {

        loopRunning = true;

        while (loopRunning)
        {

            switch (Request)
            {
                case WebRequest.Ping:
                    await PingServer();
                    break;
                case WebRequest.GetKey:
                    ServerPublicKey = await GetPublicKey();
                    return;
                case WebRequest.None:
                default:
                    if (Time.unscaledTime - timeSinceLastConnection >= pingInterval)
                        Request = WebRequest.Ping;
                    break;
            }

            if (string.IsNullOrWhiteSpace(ServerPublicKey))
                Request = WebRequest.GetKey;

            await Task.Delay(200);

        }

    }


    private async Task<string> GetPublicKey()
    {

        using UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoints[Endpoint.PublicKey]);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            return request.downloadHandler.text;
        else
            return null;

    }


    private async Task PingServer()
    {

        using UnityWebRequest request = UnityWebRequest.Head(baseURL + endpoints[Endpoint.Ping]);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Request = WebRequest.None;
        else
            connectionRunning = false;

    }


    /*

    static async Task Main(string[] args)
    {

        bool onlineMode = false;

        baseURL = onlineMode ? "https://odrestserver.onrender.com/" : "https://localhost:32771/";

        using (RSA rsa = RSA.Create())
        {

            clientPrivateKey = rsa.ToXmlString(true);
            clientPublicKey = rsa.ToXmlString(false);


        }

        var program = new Program();

        if (string.IsNullOrWhiteSpace(serverPublicKey))
            serverPublicKey = await program.GetPublicKey();

        Console.WriteLine(serverPublicKey);

        CreateUserDTO newUser = new CreateUserDTO();

        newUser.Email = "simon@test.dk";
        newUser.Name = "SimonTest";
        string password = "DetteErEnTest";

        using (RSA rsa = RSA.Create())
        {

            rsa.FromXmlString(serverPublicKey);
            newUser.Password = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(password), RSAEncryptionPadding.Pkcs1));

        }

        string response = await program.CreateUser(newUser);

        Console.WriteLine(response);

        Console.ReadLine();

        LoginDTO loginDTO = new LoginDTO();
        loginDTO.Email = "simon@test.dk";
        loginDTO.EncryptReturnKey = clientPublicKey;

        using (RSA rsa = RSA.Create())
        {

            rsa.FromXmlString(serverPublicKey);
            loginDTO.Password = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(password), RSAEncryptionPadding.Pkcs1));

        }

        UserReturnDTO returnDTO = await program.Login(loginDTO);
        string returnUser;
        string returnEmail;

        if (returnDTO == default)
            Console.WriteLine("Login failed");
        else
        {

            using (RSA rsa = RSA.Create())
            {

                rsa.FromXmlString(clientPrivateKey);
                returnUser = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(returnDTO.Name), RSAEncryptionPadding.Pkcs1));
                returnEmail = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(returnDTO.Email), RSAEncryptionPadding.Pkcs1));

            }

            Console.WriteLine($"{returnUser} with email: {returnEmail} - was successfully logged in");

        }

    }


    private async Task<string> GetPublicKey()
    {

        var response = await client.GetAsync(baseURL + endpoints[Endpoint.PublicKey]);

        return await response.Content.ReadAsStringAsync();

    }


    private async Task<string> CreateUser(CreateUserDTO newUser)
    {

        HttpResponseMessage response = await client.PostAsJsonAsync(baseURL + endpoints[Endpoint.AddUser], newUser);

        return await response.Content.ReadAsStringAsync();

    }


    private async Task<UserReturnDTO> Login(LoginDTO login)
    {

        HttpResponseMessage response = await client.PostAsJsonAsync(baseURL + endpoints[Endpoint.Login], login);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserReturnDTO>();
        else
            return null;

    }

    */

}

#region Requestforms

public enum WebRequest
{

    None,
    GetKey,
    Ping

}

#endregion
#region Endpoints

public enum Endpoint
{

    PublicKey,
    AddUser,
    Login,
    ClearUsers,
    TestReadUsers,
    AchievementsEarned,
    OwnAchievements,
    AddAchievement,
    ClearScoresAndAchievements,
    AddHighscore,
    OwnHighscore,
    Leaderboard,
    Ping

}

#endregion
#region DTOs

/// <summary>
/// Data storage class for saving relevant info pertinent for a specific user
/// </summary>
public class User
{

    public string Name { get; set; }


    public byte[] PasswordHashWithSalt { get; set; }


    public byte[] Salt { get; set; }


    public string Email { get; set; }


    public DateTime JoinTime { get; set; }

}

/// <summary>
/// Data transfer object with data needed for logging in
/// </summary>
public class LoginDTO
{


    public string Email { get; set; }


    public string Password { get; set; }


    public string EncryptReturnKey { get; set; }


}

/// <summary>
/// Data transfer object with data pertinent for creating a new user
/// </summary>
public class CreateUserDTO
{


    public string Name { get; set; }


    public string Email { get; set; }


    public string Password { get; set; }

}

/// <summary>
/// Data transfer object that's used for sending data to requestee
/// </summary>
public class UserReturnDTO
{


    public string Name { get; set; }


    public string Email { get; set; }

}

/// <summary>
/// Data storage class for saving information pertinent for a earned achievement
/// </summary>
public class Achievement
{


    public DateTime Date { get; set; }


    public string UserName { get; set; }


    public string UserEmail { get; set; }


    public int AchievementID { get; set; }

}

/// <summary>
/// Data storage class for saving information pertinent for a highscore
/// </summary>
public class HighScore
{


    public DateTime Date { get; set; }


    public string UserName { get; set; }


    public string UserEmail { get; set; }


    public int Score { get; set; }

}

#endregion
