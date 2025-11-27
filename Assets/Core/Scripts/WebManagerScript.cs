using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#region RequestForms

public enum WebRequest
{

    Idle,
    GetKey,
    Ping,
    AddHighscore,
    AddAchievement,
    CreateUser,
    Login,
    OwnAchievements,
    OwnHighscore,
    ShowAchievements,
    ShowHighscores

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
    private static bool loopRunning = false, own = false, approved = false, showHighScores = false, showAchievements = true, chatActive = false;
    private static float timeSinceLastConnectionAttempt;
    private static string serverPublicKey, clientPrivateKey, clientPublicKey;
    private static WebRequest currentRequest = WebRequest.GetKey;
    private static List<WebRequest> requests = new List<WebRequest>();
    private static UserReturnDTO currentUser = null;
    private static LoginDTO login = null;
    private static CreateUserDTO createUser = null;
    private static HighScoreDTO highScore = null;
    private static AchievementDTO achievement = null;
    private static DataTransfer_SO events;
    private static readonly object transitionLock = new object();
    private static Dictionary<Endpoint, string> endpoints = new Dictionary<Endpoint, string>
        {

            // { Endpoint.ClearUsers , "UserListing/clear" },
            // { Endpoint.TestReadUsers , "UserListing/testreader" },
            // { Endpoint.ClearScoresAndAchievements , "AchievementListing/clear" },
            { Endpoint.AddAchievement , "AchievementListing/addachievement" },
            { Endpoint.OwnAchievements , "AchievementListing/getownachievements" },
            { Endpoint.AchievementsEarned, "AchievementListing/achievementsearned" },
            { Endpoint.AddHighscore , "AchievementListing/addscore" },
            { Endpoint.OwnHighscore , "AchievementListing/getownscore" },
            { Endpoint.Leaderboard , "AchievementListing/getleaderboard" },
            { Endpoint.PublicKey , "UserListing/publickey" },
            { Endpoint.AddUser , "UserListing/add" },
            { Endpoint.Login , "UserListing/login" },
            { Endpoint.Ping , "UserListing/ping" }

        };
    private static readonly List<WebRequest> requiresData = new List<WebRequest>
    {

        WebRequest.Login,
        WebRequest.CreateUser,
        WebRequest.AddHighscore,
        WebRequest.AddAchievement,

    };

    #endregion
    #region Properties


    public static WebRequest Request
    {

        set
        {

            if ((requiresData.Contains(value) && !approved) || (currentUser == null && (value == WebRequest.OwnAchievements || value == WebRequest.OwnHighscore)))
            {

                Debug.LogError("Invalid request added, needs data");
                return;

            }

            if (value != currentRequest)
            {

                timeSinceLastConnectionAttempt = Time.unscaledTime;

                lock (transitionLock)
                    if (currentRequest == WebRequest.Idle || value == WebRequest.Idle)
                        currentRequest = value;
                    else if (!requests.Contains(value))
                        requests.Add(value);

            }

            approved = false;

        }

    }


    public static bool ShowHighscore
    {

        get => showHighScores;
        set
        {

            if (value)
                showAchievements = false;

            showHighScores = value;

        }

    }


    public static bool ShowAchievement
    {

        get => showAchievements;
        set
        {

            if (value)
                showHighScores = false;

            showAchievements = value;

        }

    }


    public static bool ChatActive
    {

        get => chatActive;
        set
        {

            if (value && !showAchievements && !showHighScores)
                showAchievements = true;

            chatActive = value;

        }

    }


    private static string ServerPublicKey
    {

        set
        {

            if (currentRequest == WebRequest.GetKey && !string.IsNullOrEmpty(value))
                Request = WebRequest.Idle;

            serverPublicKey = value;

        }

    }


    public static bool ConnectionRunning { get => lastConnectionActive; }


    private static HighScoreDTO HighScore
    {

        set
        {

            highScore = value;

            bool pending = requests.Contains(WebRequest.AddHighscore);

            lock (transitionLock)
                if (highScore == null && pending)
                    requests.Remove(WebRequest.AddHighscore);
                else if (!pending)
                    requests.Add(WebRequest.AddHighscore);

        }

    }


    private static AchievementDTO Achievement
    {

        set
        {

            achievement = value;

            bool pending = requests.Contains(WebRequest.AddAchievement);

            lock (transitionLock)
                if (achievement == null && pending)
                    requests.Remove(WebRequest.AddAchievement);
                else if (!pending)
                    requests.Add(WebRequest.AddAchievement);

        }

    }


    private static CreateUserDTO CreateUser
    {

        set
        {

            createUser = value;

            bool pending = requests.Contains(WebRequest.CreateUser);

            lock (transitionLock)
                if (createUser == null && pending)
                    requests.Remove(WebRequest.CreateUser);
                else if (!pending)
                    requests.Add(WebRequest.CreateUser);

        }

    }


    private static LoginDTO Login
    {

        set
        {

            login = value;

            bool pending = requests.Contains(WebRequest.Login);

            lock (transitionLock)
                if (login == null && pending)
                    requests.Remove(WebRequest.Login);
                else if (!pending)
                    requests.Add(WebRequest.Login);

        }

    }


    public static UserReturnDTO CurrentUser { get => currentUser; set => currentUser = value; }

    #endregion
    #region Methods


    private async void Awake()
    {

        DontDestroyOnLoad(gameObject);

        timeSinceLastConnectionAttempt = Time.unscaledTime;

        using (RSA rsa = RSA.Create())
        {

            clientPrivateKey = rsa.ToXmlString(true);
            clientPublicKey = rsa.ToXmlString(false);

        }

        if (!loopRunning)
            await TaskHandler();

    }


    private void Start()
    {

#pragma warning disable CS0219
        string name = "OceanDefenderUnityProgram", mail = "oceandefender@oceandefender.dk", password = "MortenErSejereEndDinMor"; //Testing strings
#pragma warning restore CS0219

        Login = new LoginDTO(mail, password);

    }

    private void OnEnable()
    {

        events = DataTransfer_SO.Instance;
        events.resetEvent += ClearCache;

    }


    private void OnDisable()
    {

        loopRunning = false;
        events.resetEvent -= ClearCache;

    }


    public static void RequestWithData<T>(T obj) where T : ISendableDTO
    {

        switch (obj)
        {
            case CreateUserDTO createUserDTO when !string.IsNullOrWhiteSpace(createUserDTO.Password) && !string.IsNullOrWhiteSpace(createUserDTO.Email) && !string.IsNullOrWhiteSpace(createUserDTO.Name):
                CreateUser = createUserDTO;
                break;
            case LoginDTO loginDTO when !string.IsNullOrWhiteSpace(loginDTO.Email) && !string.IsNullOrWhiteSpace(loginDTO.Password):
                loginDTO.EncryptReturnKey = clientPublicKey;
                Login = loginDTO;
                break;
            case AchievementDTO achievementDTO when !string.IsNullOrWhiteSpace(achievementDTO.UserName) && !string.IsNullOrWhiteSpace(achievementDTO.UserEmail):
                achievementDTO.Date = DateTime.UtcNow;
                Achievement = achievementDTO;
                break;
            case HighScoreDTO highScoreDTO when !string.IsNullOrWhiteSpace(highScoreDTO.UserName) && !string.IsNullOrWhiteSpace(highScoreDTO.UserEmail) && highScoreDTO.Score > 0:
                highScoreDTO.Date = DateTime.UtcNow;
                HighScore = highScoreDTO;
                break;
            default:
                Debug.LogError("RequestWithData object contained/was null data, or missing handle logic");
                break;
        }

    }


    private async Task TaskHandler()
    {

        loopRunning = true;

        while (loopRunning)
        {

            try
            {

                if (currentRequest == WebRequest.Idle && requests.Count > 0)
                {

                    lock (transitionLock)
                    {

                        approved = true;
                        Request = requests[0];
                        requests.RemoveAt(0);

                    }

                }

                object needsAttention = null;

                switch (currentRequest)
                {
                    case WebRequest.Ping:
                        await PingServer();
                        break;
                    case WebRequest.GetKey:
                        ServerPublicKey = await GetPublicKey();
                        break;
                    case WebRequest.CreateUser:
                        await CreateUserPost();
                        break;
                    case WebRequest.Login:
                        needsAttention = await PostLogin();
                        break;
                    case WebRequest.AddHighscore:
                        await PostHighscore();
                        break;
                    case WebRequest.AddAchievement:
                        await PostAchievement();
                        break;
                    case WebRequest.OwnAchievements:
                        needsAttention = await PostGetOwnAchievement();
                        break;
                    case WebRequest.OwnHighscore:
                        needsAttention = await PostGetOwnHighscore();
                        break;
                    case WebRequest.ShowAchievements:
                        needsAttention = await GetLastAchievements();
                        break;
                    case WebRequest.ShowHighscores:
                        needsAttention = await GetLeaderboard();
                        break;
                    case WebRequest.Idle:
                    default:
                        if (Time.unscaledTime - timeSinceLastConnectionAttempt >= pingInterval && !requests.Contains(WebRequest.Ping))
                            switch (chatActive)
                            {
                                case true when showAchievements:
                                    Request = WebRequest.ShowAchievements;
                                    break;
                                case true when showHighScores:
                                    Request = WebRequest.ShowHighscores;
                                    break;
                                default:
                                    Request = WebRequest.Ping;
                                    break;
                            }
                        break;
                }

                if (needsAttention != null)
                    ObjectHandler(needsAttention);

                if (string.IsNullOrWhiteSpace(serverPublicKey) && currentRequest == WebRequest.Idle && !requests.Contains(WebRequest.GetKey))
                    Request = WebRequest.GetKey;

                await Task.Delay(200);

            }
            catch (Exception e)
            {

                Debug.LogError(e);

            }

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
                userReturnDTO.Email = Decrypt(userReturnDTO.Email);
                userReturnDTO.Name = Decrypt(userReturnDTO.Name);
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
                Debug.LogError("Unknown DTO in ObjectHandler");
                break;
        }

    }


    public static void ClearCache()
    {

        currentUser = null;
        login = null;
        createUser = null;
        highScore = null;
        achievement = null;

    }


    private string Decrypt(string input)
    {

        byte[] bytes = Convert.FromBase64String(input);

        using (RSA rsa = RSA.Create())
        {

            rsa.FromXmlString(clientPrivateKey);
            return Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1));

        }

    }


    private string Encrypt(string input)
    {

        byte[] bytes = Encoding.UTF8.GetBytes(input);

        using (RSA rsa = RSA.Create())
        {

            rsa.FromXmlString(serverPublicKey);
            return Convert.ToBase64String(rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1));

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
            lastConnectionActive = true;
        else
            lastConnectionActive = false;

        Request = WebRequest.Idle;

    }


    private async Task CreateUserPost()
    {

        if (createUser == null)
        {

            Debug.LogWarning("createUser was null");
            return;

        }

        createUser.Password = Encrypt(createUser.Password);

        string json = JsonConvert.SerializeObject(createUser);
        createUser = null;
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.AddUser], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                Debug.Log(request.downloadHandler.text);

            }
            else
            {

                Debug.LogError("Error creating user");
                lastConnectionActive = false;

            }

            Request = WebRequest.Idle;

        }

    }


    private async Task<UserReturnDTO> PostLogin()
    {

        if (login == null)
        {

            Debug.LogWarning("login credentials were null");
            Request = WebRequest.Idle;
            return null;

        }

        login.Password = Encrypt(login.Password);

        string json = JsonConvert.SerializeObject(login);
        login = null;
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.Login], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                Debug.Log(request.downloadHandler.text);
                Request = WebRequest.Idle;
                return JsonConvert.DeserializeObject<UserReturnDTO>(request.downloadHandler.text);

            }
            else
            {

                Debug.LogError("Login Error");
                lastConnectionActive = false;

            }

        }

        Request = WebRequest.Idle;
        return null;

    }


    private async Task PostHighscore()
    {

        if (highScore == null)
        {

            Debug.LogWarning("highscore was null");
            return;

        }

        string json = JsonConvert.SerializeObject(highScore);
        highScore = null;
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.AddHighscore], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                Debug.Log(request.downloadHandler.text);

            }
            else
            {

                Debug.LogError("Error posting highscore");
                lastConnectionActive = false;

            }

            Request = WebRequest.Idle;

        }

    }


    private async Task PostAchievement()
    {

        if (achievement == null)
        {

            Debug.LogWarning("achievement was null");
            return;

        }

        string json = JsonConvert.SerializeObject(achievement);
        achievement = null;
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.AddAchievement], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                Debug.Log(request.downloadHandler.text);

            }
            else
            {

                Debug.LogError("Error posting achievement");
                lastConnectionActive = false;

            }

            Request = WebRequest.Idle;

        }

    }


    private async Task<List<AchievementDTO>> PostGetOwnAchievement()
    {

        if (currentUser == null)
        {

            Debug.Log("No user logged in");
            Request = WebRequest.Idle;
            return null;

        }

        string json = JsonConvert.SerializeObject(currentUser);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.OwnAchievements], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            own = true;

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                Debug.Log(request.downloadHandler.text);
                Request = WebRequest.Idle;
                return JsonConvert.DeserializeObject<List<AchievementDTO>>(request.downloadHandler.text);

            }
            else
            {

                Debug.LogError("Error getting own achievements");
                lastConnectionActive = false;

            }

        }

        Request = WebRequest.Idle;
        return null;

    }


    private async Task<List<AchievementDTO>> GetLastAchievements()
    {

        using UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoints[Endpoint.AchievementsEarned]);
        await request.SendWebRequest();

        Request = WebRequest.Idle;

        if (request.result == UnityWebRequest.Result.Success)
        {

            lastConnectionActive = true;
            return JsonConvert.DeserializeObject<List<AchievementDTO>>(request.downloadHandler.text);

        }
        else
        {

            lastConnectionActive = false;
            return null;

        }

    }


    private async Task<HighScoreDTO> PostGetOwnHighscore()
    {

        if (currentUser == null)
        {

            Debug.Log("No user logged in");
            Request = WebRequest.Idle;
            return null;

        }

        string json = JsonConvert.SerializeObject(currentUser);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.OwnHighscore], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                Debug.Log(request.downloadHandler.text);
                Request = WebRequest.Idle;
                return JsonConvert.DeserializeObject<HighScoreDTO>(request.downloadHandler.text);

            }
            else
            {

                Debug.LogError("Error getting own highscore");
                lastConnectionActive = false;

            }

        }

        Request = WebRequest.Idle;
        return null;

    }


    private async Task<List<HighScoreDTO>> GetLeaderboard()
    {

        using UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoints[Endpoint.Leaderboard]);
        await request.SendWebRequest();

        Request = WebRequest.Idle;

        if (request.result == UnityWebRequest.Result.Success)
        {

            lastConnectionActive = true;
            return JsonConvert.DeserializeObject<List<HighScoreDTO>>(request.downloadHandler.text);

        }
        else
        {

            lastConnectionActive = false;
            return null;

        }

    }

    #endregion
    #endregion

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
public class LoginDTO : ISendableDTO
{


    public string Email { get; set; }


    public string Password { get; set; }


    public string EncryptReturnKey { get; set; }


    public LoginDTO(string email, string password)
    {

        Email = email;
        Password = password;

    }

}

/// <summary>
/// Data transfer object with data pertinent for creating a new user
/// </summary>
public class CreateUserDTO : ISendableDTO
{


    public string Name { get; set; }


    public string Email { get; set; }


    public string Password { get; set; }


    public CreateUserDTO(string name, string email, string password)
    {

        Name = name;
        Email = email;
        Password = password;

    }

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
public class AchievementDTO : ISendableDTO
{


    public DateTime Date { get; set; }


    public string UserName { get; set; }


    public string UserEmail { get; set; }


    public int AchievementID { get; set; }


    public AchievementDTO(string userName, string userEmail, int achievementID)
    {

        UserName = userName;
        UserEmail = userEmail;
        AchievementID = achievementID;

    }
}

/// <summary>
/// Data storage class for saving information pertinent for a highscore
/// </summary>
public class HighScoreDTO : ISendableDTO
{


    public DateTime Date { get; set; }


    public string UserName { get; set; }


    public string UserEmail { get; set; }


    public int Score { get; set; }


    public HighScoreDTO(string userName, string userEmail, int score)
    {

        UserName = userName;
        UserEmail = userEmail;
        Score = score;

    }

}


public interface ISendableDTO
{

}

#endregion
