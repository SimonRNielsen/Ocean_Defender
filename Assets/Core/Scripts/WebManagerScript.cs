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

    private static readonly float pingInterval = 20f;
    private static readonly string baseURL = "https://odrestserver.onrender.com/";
    private static bool loopRunning = false, own = false, approved = false, showHighScores = false, showAchievements = true, chatActive = false, lastConnectionActive = false;
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
    private static readonly Dictionary<Endpoint, string> endpoints = new Dictionary<Endpoint, string>
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

    /// <summary>
    /// Set to request data from server except for requests needing data (Create user, Login, Add Achievement, Add Highscore) - use "RequestWithData" for this instead
    /// </summary>
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

                if (value != WebRequest.Idle)
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

    /// <summary>
    /// Set if chat is set to show leaderboard
    /// </summary>
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

    /// <summary>
    /// Set if chat is set to show achievements
    /// </summary>
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

    /// <summary>
    /// Set if "chat" is active
    /// </summary>
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

    /// <summary>
    /// Servers key for encrypting the message sent to it with its private key (RSA)
    /// </summary>
    private static string ServerPublicKey
    {

        set
        {

            if (currentRequest == WebRequest.GetKey && !string.IsNullOrEmpty(value))
                Request = WebRequest.Idle;

            serverPublicKey = value;

        }

    }

    /// <summary>
    /// Checks if last connection was successful
    /// </summary>
    public static bool ConnectionRunning { get => lastConnectionActive; }

    /// <summary>
    /// Datatransfer object for POSTing a highscore, queues request automatically
    /// </summary>
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

    /// <summary>
    /// Datatransfer object for POSTing an achievement, queues request automatically
    /// </summary>
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

    /// <summary>
    /// Datatransfer object for POSTing a new user, queues request automatically
    /// </summary>
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

    /// <summary>
    /// Datatransfer object for POSTing a login request, queues request automatically
    /// </summary>
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

    /// <summary>
    /// Info on currently logged in user
    /// </summary>
    public static UserReturnDTO CurrentUser { get => currentUser; }

    #endregion
    #region Methods

    /// <summary>
    /// Initiates service at startup
    /// </summary>
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

    /// <summary>
    /// Currently only used for testing
    /// </summary>
    private void Start()
    {

        //string name = "OceanDefenderUnityProgram", mail = "oceandefender@oceandefender.dk", password = "MortenErSejereEndDinMor"; //Testing strings

    }

    /// <summary>
    /// Used for initating connection and reset subscriptions on SO
    /// </summary>
    private void OnEnable()
    {

        events = DataTransfer_SO.Instance;
        events.resetEvent += ClearCache;

    }

    /// <summary>
    /// Used for shutting down safely
    /// </summary>
    private void OnDisable()
    {

        loopRunning = false;
        events.resetEvent -= ClearCache;

    }

    /// <summary>
    /// Method to send requests needing data (Create user, Login, Add Achievement, Add Highscore)
    /// </summary>
    /// <typeparam name="T">Generic object type</typeparam>
    /// <param name="obj">Object needed for sending requests</param>
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

    /// <summary>
    /// Handles running and dequeuing tasks
    /// </summary>
    /// <returns>Handled task</returns>
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
                        Request = requests[0]; //Alternative combine with RemoveAt to a Queue<WebRequest> since that can't check or remove content other than next in line
                        requests.RemoveAt(0);

                    }

                }

                object needsAttention = null; //Used for generic object handling

                switch (currentRequest) //State machine for requests
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

                if (string.IsNullOrWhiteSpace(serverPublicKey) && currentRequest == WebRequest.Idle && !requests.Contains(WebRequest.GetKey)) //Backup in case encryption key isn't recieved
                    Request = WebRequest.GetKey;

                await Task.Delay(200); //Ensures server and async runtime isn't swamped by requests

            }
            catch (Exception e)
            {

                Debug.LogError(e);

            }

        }

    }

    /// <summary>
    /// Handles centralized logic for what to do with recieved reply from server dependant on what the reply is
    /// </summary>
    /// <param name="obj">Generic object for polymorphism</param>
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
            case UserReturnDTO userReturnDTO when !(string.IsNullOrEmpty(userReturnDTO.Name) || string.IsNullOrEmpty(userReturnDTO.Email)):
                userReturnDTO.Email = Decrypt(userReturnDTO.Email);
                userReturnDTO.Name = Decrypt(userReturnDTO.Name);
                currentUser = userReturnDTO;
                Debug.Log($"User: {currentUser.Name}. With email: {currentUser.Email} has been logged in");
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
            case List<HighScoreDTO> emptyHighscores when emptyHighscores.Count == 0:
            case List<AchievementDTO> emptyAchievements when emptyAchievements.Count == 0:
                Debug.LogWarning("Request recieved empty list");
                break;
            default:
                Debug.LogError($"DTO with invalid data caught in ObjectHandler {obj}");
                break;
        }

    }

    /// <summary>
    /// Clears data of any DTOs or logged in user - can also be triggeded with DataTransfer_SO.resetEvent?.Invoke()
    /// </summary>
    public static void ClearCache()
    {

        currentUser = null;
        login = null;
        createUser = null;
        highScore = null;
        achievement = null;

    }

    /// <summary>
    /// Decrypts a string formatted for transmission via Json/RESTful server
    /// </summary>
    /// <param name="input">Encrypted string from server</param>
    /// <returns>Decrypted string</returns>
    private string Decrypt(string input)
    {

        byte[] bytes = Convert.FromBase64String(input);

        using (RSA rsa = RSA.Create())
        {

            rsa.FromXmlString(clientPrivateKey);
            return Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1));

        }

    }

    /// <summary>
    /// Encrypts and formats string for serialization into Json format to POSTing @ server
    /// </summary>
    /// <param name="input">String that must be encrypted using the servers public key</param>
    /// <returns>Encrypted and formatted string</returns>
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

    /// <summary>
    /// Sends a GET request for servers public RSA encryption key used for asyncronous secure transmission of password (even though HTTPS already does this, allows somewhat safe deployment on HTTP though)
    /// </summary>
    /// <returns>Public RSA key from server</returns>
    private async Task<string> GetPublicKey()
    {

        using UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoints[Endpoint.PublicKey]);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            lastConnectionActive = true;
            return request.downloadHandler.text;

        }
        else if (request.result == UnityWebRequest.Result.ConnectionError)
        {

            lastConnectionActive = false;
            Debug.LogError("No connection to server");

        }
        else
        {

            Debug.LogWarning("Error getting key");
            lastConnectionActive = true;

        }

        return null;

    }

    /// <summary>
    /// Regular ping method to ensure there's a stable and valid connection to server send as a HEAD (also used by monitoring site)
    /// </summary>
    /// <returns>Only possible reply is a "Ok" status from server</returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.LogError("No connection to server");
                lastConnectionActive = false;

            }
            else
            {

                Debug.Log("From server: " + request.downloadHandler.text);
                lastConnectionActive = true;

            }

            Request = WebRequest.Idle;

        }

    }

    /// <summary>
    /// Handles a login via a POST request (post sends a LoginDTO)
    /// </summary>
    /// <returns>Returns a UserReturnDTO if successful</returns>
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
        Request = WebRequest.Idle;
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + endpoints[Endpoint.Login], "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(jsonBytes) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                return JsonConvert.DeserializeObject<UserReturnDTO>(request.downloadHandler.text);

            }
            else if (request.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.LogError("No connection to server");
                lastConnectionActive = false;

            }
            else
            {

                Debug.LogWarning("From server: " + request.downloadHandler.text);
                lastConnectionActive = true;

            }

        }

        return null;

    }

    /// <summary>
    /// Sends a POST request to server with a HighscoreDTO
    /// </summary>
    /// <returns></returns>
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

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.LogError("No connection to server");
                lastConnectionActive = false;

            }
            else
            {

                Debug.Log("From server: " + request.downloadHandler.text);
                lastConnectionActive = true;

            }

            Request = WebRequest.Idle;

        }

    }

    /// <summary>
    /// Sends a POST request to server with a AchievementDTO
    /// </summary>
    /// <returns>Message -> debug, from server if successful</returns>
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

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.LogError("No connection to server");
                lastConnectionActive = false;

            }
            else
            {

                Debug.Log("From server: " + request.downloadHandler.text);
                lastConnectionActive = true;

            }

            Request = WebRequest.Idle;

        }

    }

    /// <summary>
    /// Sends a POST request with current users info that gets a List of logged in users own achievements
    /// </summary>
    /// <returns>Returns a List of AchievementDTO on a success</returns>
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
            Request = WebRequest.Idle;

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                return JsonConvert.DeserializeObject<List<AchievementDTO>>(request.downloadHandler.text);

            }
            else if (request.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.LogError("No connection to server");
                lastConnectionActive = false;

            }
            else
            {

                Debug.LogWarning("From server: " + request.downloadHandler.text);
                lastConnectionActive = true;

            }

        }

        return null;

    }

    /// <summary>
    /// Sends at GET request to server for the 10 last earned achievements globally
    /// </summary>
    /// <returns>Returns a List of AchievementDTOs on success</returns>
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
        else if (request.result == UnityWebRequest.Result.ConnectionError)
        {

            Debug.LogError("No connection to server");
            lastConnectionActive = false;

        }
        else
        {

            Debug.LogWarning("From server: " + request.downloadHandler.text);
            lastConnectionActive = true;

        }

        return null;

    }

    /// <summary>
    /// Sends a POST request with current users information that gets users highscore
    /// </summary>
    /// <returns>Returns a single HighScoreDTO on success</returns>
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
            Request = WebRequest.Idle;

            if (request.result == UnityWebRequest.Result.Success)
            {

                lastConnectionActive = true;
                return JsonConvert.DeserializeObject<HighScoreDTO>(request.downloadHandler.text);

            }
            else if (request.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.LogError("No connection to server");
                lastConnectionActive = false;

            }
            else
            {

                Debug.LogWarning("From server: " + request.downloadHandler.text);
                lastConnectionActive = false;

            }

        }

        return null;

    }

    /// <summary>
    /// Sends a GET request to server that returns the highscore-leaderboard (Top 10 global scores)
    /// </summary>
    /// <returns>Returns a List of HighScoreDTOs on success</returns>
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
        else if (request.result == UnityWebRequest.Result.ConnectionError)
        {

            Debug.LogError("No connection to server");
            lastConnectionActive = false;

        }
        else
        {

            Debug.LogWarning("From server: " + request.downloadHandler.text);
            lastConnectionActive = true;

        }

        return null;

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
