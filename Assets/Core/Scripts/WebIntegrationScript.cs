using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebIntegrationScript : MonoBehaviour
{

    [SerializeField] private bool simulated = true;
    [SerializeField] private string onlineUrl = "https://odrestserver.onrender.com/", offlineUrl = "http://10.131.66.121:32771/";
    private static bool connectionRunning = false;
    private static string serverPublicKey, clientPrivateKey, clientPublicKey;
    private static string baseURL;
    private static Thread connectionThread;
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


    public static WebRequest Request { get => request; set => request = value; }


    public static string ServerPublicKey 
    { 

        get => serverPublicKey;  
        set
        {

            if (value != serverPublicKey)
                Debug.LogWarning(value);

            serverPublicKey = value;

        }

    }

    private void Awake()
    {

        baseURL = simulated ? offlineUrl : onlineUrl;
        connectionThread = new Thread(RunThread);
        connectionThread.IsBackground = true;
        connectionThread.Start();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

        //serverPublicKey = await GetPublicKey();

    }

    // Update is called once per frame
    void Update()
    {

    }


    public async Task<string> GetPublicKey()
    {

        using UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoints[Endpoint.PublicKey]);
        request.certificateHandler = new AcceptAllCertificates();
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            return request.downloadHandler.text;
        else
            return null;

    }


    public async void RunThread()
    {

        while (true)
        {

            switch (request)
            {
                case WebRequest.GetKey:
                    ServerPublicKey = await GetPublicKey();
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(ServerPublicKey))
                Request = WebRequest.GetKey;

        }

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

public enum WebRequest
{

    None,
    GetKey

}


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

class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}
