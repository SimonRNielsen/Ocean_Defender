using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


#region RequestForms

public enum WebRequest
{

    None,
    GetKey,
    Ping,
    AddHighscore,
    AddAchievement,
    CreateUser,
    Login

}

#endregion

public class WebManagerScript : MonoBehaviour
{

    #region Fields

    private static readonly float pingInterval = 60f;
    private static readonly string baseURL = "https://odrestserver.onrender.com/";
#pragma warning disable CS0414
    private static bool lastConnectionActive = false;
#pragma warning restore CS0414
    private static bool loopRunning = false, own = false;
    private static float timeSinceLastConnectionAttempt;
    private static string serverPublicKey, clientPrivateKey, clientPublicKey;
    private static Queue<WebRequest> requests = new Queue<WebRequest>();
    private static WebRequest currentRequest = WebRequest.GetKey;
    private static UserReturnDTO currentUser = null;
    private static LoginDTO login = null;
    private static CreateUserDTO createUser = null;
    private static HighScoreDTO highScore = null;
    private static AchievementDTO achievement = null;
    private static DataTransfer_SO events;
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

    #endregion
    #region Properties


    private static WebRequest Request
    {

        set
        {

            if (value != currentRequest)
            {

                timeSinceLastConnectionAttempt = Time.unscaledTime;
                if (currentRequest == WebRequest.None)
                    currentRequest = value;
                else if (!requests.Contains(value))
                    requests.Enqueue(value);

            }

        }

    }


    private static string ServerPublicKey
    {

        get => serverPublicKey;
        set
        {

            if (currentRequest == WebRequest.GetKey && !string.IsNullOrEmpty(value))
                currentRequest = WebRequest.None;

            serverPublicKey = value;

        }

    }


    public static bool ConnectionRunning { get => lastConnectionActive; }


    public static HighScoreDTO HighScore
    {

        set
        {

            highScore = value;
            Request = WebRequest.AddHighscore;

        }

    }


    public static AchievementDTO Achievement
    {

        set
        {

            achievement = value;
            Request = WebRequest.AddAchievement;

        }

    }


    public static CreateUserDTO CreateUser
    {

        set
        {

            createUser = value;
            Request = WebRequest.CreateUser;

        }

    }


    public static LoginDTO Login
    {

        set
        {

            login = value;
            Request = WebRequest.Login;

        }

    }

    #endregion
    #region Methods


    private async void Awake()
    {

        timeSinceLastConnectionAttempt = Time.unscaledTime;

        using (RSA rsa = RSA.Create())
        {

            clientPrivateKey = rsa.ToXmlString(true);
            clientPublicKey = rsa.ToXmlString(false);

        }

        if (!loopRunning)
            try
            {

                await TaskHandler();

            }
            catch (Exception e)
            {

                Debug.LogError(e);

            }

    }


    private void Start()
    {

        events = DataTransfer_SO.Instance;

    }


    private async Task TaskHandler()
    {

        loopRunning = true;

        while (loopRunning)
        {

            if (currentRequest == WebRequest.None && requests.Count > 0)
                Request = requests.Dequeue();

            object needsAttention = null;

            switch (currentRequest)
            {
                case WebRequest.Ping:
                    await PingServer();
                    break;
                case WebRequest.GetKey:
                    ServerPublicKey = await GetPublicKey();
                    break;
                case WebRequest.None:
                default:
                    if (Time.unscaledTime - timeSinceLastConnectionAttempt >= pingInterval && !requests.Contains(WebRequest.Ping))
                        Request = WebRequest.Ping;
                    break;
            }

            if (needsAttention != null)
                ObjectHandler(needsAttention);

            if (string.IsNullOrWhiteSpace(ServerPublicKey) && !requests.Contains(WebRequest.GetKey))
                Request = WebRequest.GetKey;

            await Task.Delay(200);

        }

    }


    private void ObjectHandler(object obj)
    {

        switch (obj)
        {
            case string message:
                Debug.LogWarning(message);
                break;
            case List<HighScoreDTO> leaderboard when leaderboard.Count > 0:
                events.transmitScores?.Invoke(leaderboard);
                break;
            case HighScoreDTO ownScore when ownScore.Score > 0:
                events.transmitScore?.Invoke(ownScore);
                break;
            case UserReturnDTO userReturnDTO:
                currentUser = userReturnDTO;
                break;
            case List<AchievementDTO> achievementDTOs when achievementDTOs.Count > 0:
                if (own)
                {

                    own = false;
                    events.transmitOwnAchievements?.Invoke(achievementDTOs);

                }
                else
                    events.transmitAchievements?.Invoke(achievementDTOs);
                break;
            default:
                break;
        }

    }

    #region Tasks


    private async Task<string> GetPublicKey()
    {

        using UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoints[Endpoint.PublicKey]);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            lastConnectionActive = true;
            return request.downloadHandler.text;

        }
        else
        {

            lastConnectionActive = false;
            return null;

        }

    }


    private async Task PingServer()
    {

        using UnityWebRequest request = UnityWebRequest.Head(baseURL + endpoints[Endpoint.Ping]);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            currentRequest = WebRequest.None;
            lastConnectionActive = true;

        }
        else
            lastConnectionActive = false;

    }

    #endregion
    #endregion

    /*

    static async Task Main(string[] args)
    {

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
public class AchievementDTO
{


    public DateTime Date { get; set; }


    public string UserName { get; set; }


    public string UserEmail { get; set; }


    public int AchievementID { get; set; }

}

/// <summary>
/// Data storage class for saving information pertinent for a highscore
/// </summary>
public class HighScoreDTO
{


    public DateTime Date { get; set; }


    public string UserName { get; set; }


    public string UserEmail { get; set; }


    public int Score { get; set; }

}

#endregion
