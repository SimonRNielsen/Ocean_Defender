using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginUIScript : MonoBehaviour
{
    #region Fields
    private VisualElement root;

    //Buttons for openeing screens
    private Button loginButton;
    private Button createUserButton;
    private Button logOutButton;
    private Button showHighscoreButton;
    private Button postButton, ownStatsHighscoreScreenButton, ownStatsAchievmentScreenButton;

    //LoginScreen Fields
    private Button closeLoginButton;
    private Button sendLoginButton;
    private TextField loginEmailTextField;
    private TextField loginPasswordTextField;
    private VisualElement loginScreen;

    //CreateUser Fields
    private Button closeCreateUserButton;
    private Button sendCreateUserButton;
    private TextField createUserEmailTextField;
    private TextField createUserPasswordTextField;
    private TextField createUserUserNameTextField;
    private VisualElement createUserScreen;

    //HighScore Fields
    private Button closeHighscoreButton;
    private Button switchToAchievementButton;
    private Label highscoreContentLabel;
    private VisualElement highscoreScreen;

    //Achievement Fields
    private Button closeAchievementButton;
    private Button switchToHighscoreButton;
    private Label achievementContentLabel;
    private VisualElement achievementScreen;

    private bool loggedIn = false;
    private List<VisualElement> screens = new List<VisualElement>();

    private static bool updatedScores = false;
    private int ownScore;
    private bool showOwn = false;
    private List<AchievementDTO> ownAchievements = null;
    #endregion
    #region Properties

    public static bool UpdatedScores { get => updatedScores; set => updatedScores = value; }

    #endregion
    #region Methods

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        //Buttons to upon screens
        loginButton = root.Q<Button>("LoginButton");
        createUserButton = root.Q<Button>("CreateUserButton");
        logOutButton = root.Q<Button>("LogOutButton");
        showHighscoreButton = root.Q<Button>("ShowHighscoreButton");
        postButton = root.Q<Button>("PostButton");

        //Login elements
        sendLoginButton = root.Q<Button>("SendLoginButton");
        loginEmailTextField = root.Q<TextField>("LoginEmailTextField");
        loginPasswordTextField = root.Q<TextField>("LoginPasswordTextField");
        loginScreen = root.Q<VisualElement>("LoginScreen");
        closeLoginButton = root.Q<Button>("CloseLoginButton");

        //CreateUserElements
        sendCreateUserButton = root.Q<Button>("SendCreateUserButton");
        createUserEmailTextField = root.Q<TextField>("CreateUserEmailTextField");
        createUserPasswordTextField = root.Q<TextField>("CreateUserPasswordTextField");
        createUserUserNameTextField = root.Q<TextField>("CreateUserUserNameTextField");
        createUserScreen = root.Q<VisualElement>("CreateUserScreen");
        closeCreateUserButton = root.Q<Button>("CloseCreateUserButton");

        //Highscore elements:
        closeHighscoreButton = root.Q<Button>("CloseHighscoreButton");
        switchToAchievementButton = root.Q<Button>("SwitchToAchievementButton");
        highscoreContentLabel = root.Q<Label>("HighScoreContentLabel");
        highscoreScreen = root.Q<VisualElement>("HighscoreScreen");
        ownStatsHighscoreScreenButton = root.Q<Button>("OwnStatsHighscoreScreenButton");

        //Achievement elements:
        closeAchievementButton = root.Q<Button>("CloseAchievementButton");
        switchToHighscoreButton = root.Q<Button>("SwitchToHighscoreButton");
        achievementContentLabel = root.Q<Label>("AchievementContentLabel");
        achievementScreen = root.Q<VisualElement>("AchievementScreen");
        ownStatsAchievmentScreenButton = root.Q<Button>("OwnStatsAchievementScreenButton");

        //Add screens to screen list, for easy acces to loop through them
        screens.AddRange(new VisualElement[] { loginScreen, createUserScreen, highscoreScreen, achievementScreen });
    }

    private void OnEnable()
    {
        loginButton.clicked += OnLoginButtonClicked;
        createUserButton.clicked += OnCreateUserButtonClicked;
        logOutButton.clicked += OnLogOutButtonClicked;
        sendLoginButton.clicked += OnSendLoginButtonClicked;
        closeLoginButton.clicked += OnCloseLoginButtonClicked;
        sendCreateUserButton.clicked += OnSendCreateUserButtonClicked;
        closeCreateUserButton.clicked += OnCloseCreateUserButtonClicked;
        showHighscoreButton.clicked += OnShowHighscoreButtonClicked;
        closeHighscoreButton.clicked += OnCloseHighScoreButtonClicked;
        switchToAchievementButton.clicked += OnSwitchToAchievementButtonClicked;
        closeAchievementButton.clicked += OnCloseAchievementButtonClicked;
        switchToHighscoreButton.clicked += OnSwitchToHighScoreButtonClicked;
        postButton.clicked += PublishHighscoreAndAchievements;
        DataTransfer_SO.Instance.transmitAchievements += SetAchievementsField;
        DataTransfer_SO.Instance.transmitScores += SetLeaderboardField;
        DataTransfer_SO.Instance.transmitScore += SetScoreLabel;
        DataTransfer_SO.Instance.transmitOwnAchievements += ShowOwnAchievements;
        DataTransfer_SO.Instance.resetEvent += ResetLocalData;
        ownStatsAchievmentScreenButton.clicked += ShowOwnStatsToggle;
        ownStatsHighscoreScreenButton.clicked += ShowOwnStatsToggle;
    }


    private void OnDisable()
    {
        loginButton.clicked -= OnLoginButtonClicked;
        createUserButton.clicked -= OnCreateUserButtonClicked;
        logOutButton.clicked -= OnLogOutButtonClicked;
        sendLoginButton.clicked -= OnSendLoginButtonClicked;
        closeLoginButton.clicked -= OnCloseLoginButtonClicked;
        sendCreateUserButton.clicked -= OnSendCreateUserButtonClicked;
        closeCreateUserButton.clicked -= OnCloseCreateUserButtonClicked;
        showHighscoreButton.clicked -= OnShowHighscoreButtonClicked;
        closeHighscoreButton.clicked -= OnCloseHighScoreButtonClicked;
        switchToAchievementButton.clicked -= OnSwitchToAchievementButtonClicked;
        closeAchievementButton.clicked -= OnCloseAchievementButtonClicked;
        switchToHighscoreButton.clicked -= OnSwitchToHighScoreButtonClicked;
        postButton.clicked -= PublishHighscoreAndAchievements;
        DataTransfer_SO.Instance.transmitAchievements -= SetAchievementsField;
        DataTransfer_SO.Instance.transmitScores -= SetLeaderboardField;
        DataTransfer_SO.Instance.transmitScore -= SetScoreLabel;
        DataTransfer_SO.Instance.transmitOwnAchievements -= ShowOwnAchievements;
        DataTransfer_SO.Instance.resetEvent -= ResetLocalData;
        ownStatsAchievmentScreenButton.clicked -= ShowOwnStatsToggle;
        ownStatsHighscoreScreenButton.clicked -= ShowOwnStatsToggle;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Checks whether the a user is logged in an sets loggedIn bool accordingly
        if (WebManagerScript.CurrentUser == null && loggedIn)
        {
            loggedIn = false;
        }
        else if (WebManagerScript.CurrentUser != null && !loggedIn)
        {
            loggedIn = true;
        }

        string text = string.Empty;

        if (ownScore != 0)
            text = $"Din højeste score er {ownScore}\n";
        if (ownAchievements != null && ownAchievements.Count > 0)
        {

            foreach (var achievement in ownAchievements)
                text += $"Du har optjent {(Achievements)achievement.AchievementID}\n";

        }
        if (showOwn)
        {

            achievementContentLabel.text = text;
            highscoreContentLabel.text = text;

        }

        //Uses loggedIn bool to set wheter login or logout button shoult be displayed.
        if (loggedIn)
        {
            if (loginButton.enabledInHierarchy == true)
            {
                loginButton.style.display = DisplayStyle.None;
                loginButton.SetEnabled(false);
            }
            if (createUserButton.enabledInHierarchy == true)
            {
                createUserButton.style.display = DisplayStyle.None;
                createUserButton.SetEnabled(false);
            }
            if (postButton.enabledInHierarchy == false)
            {
                postButton.SetEnabled(true);
                postButton.style.display = DisplayStyle.Flex;
            }
            if (logOutButton.enabledInHierarchy == false)
            {
                logOutButton.SetEnabled(true);
                logOutButton.style.display = DisplayStyle.Flex;
            }
            if (showHighscoreButton.enabledInHierarchy == false)
            {
                showHighscoreButton.SetEnabled(true);
                showHighscoreButton.style.display = DisplayStyle.Flex;
            }
        }
        else
        {
            if (loginButton.enabledInHierarchy == false)
            {
                loginButton.SetEnabled(true);
                loginButton.style.display = DisplayStyle.Flex;
            }
            if (createUserButton.enabledInHierarchy == false)
            {
                createUserButton.SetEnabled(true);
                createUserButton.style.display = DisplayStyle.Flex;
            }
            if (logOutButton.enabledInHierarchy == true)
            {
                logOutButton.style.display = DisplayStyle.None;
                logOutButton.SetEnabled(false);
            }
            if (showHighscoreButton.enabledInHierarchy == true)
            {
                showHighscoreButton.style.display = DisplayStyle.None;
                showHighscoreButton.SetEnabled(false);
            }
            if (postButton.enabledInHierarchy == true)
            {
                postButton.SetEnabled(false);
                postButton.style.display = DisplayStyle.None;
            }
        }
    }

    /// <summary>
    /// Hides the StartMenu in the scene
    /// </summary>
    /// <param name="hideStartMenu">Bool: whether the menu should be hidden (true) or shown (false)</param>
    public void HideStartMenu(bool hideStartMenu)
    {
        if (hideStartMenu)
        {
            FindAnyObjectByType<StartMenuScript>().GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        }
        else
        {
            FindAnyObjectByType<StartMenuScript>().GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        }

    }

    /// <summary>
    /// Hides and disables/Shows and enables a VisuaElement
    /// </summary>
    /// <param name="element">The VisualElement thatshould be affected</param>
    /// <param name="hideElement">Whether the element should be hidden(true) or shown (false)</param>
    public void HideVisualElement(VisualElement element, bool hideElement)
    {
        if (hideElement)
        {
            element.style.display = DisplayStyle.None;
            element.SetEnabled(false);
        }
        else
        {
            element.SetEnabled(true);
            element.style.display = DisplayStyle.Flex;
        }
    }

    private void OnCloseLoginButtonClicked()
    {
        loginScreen.style.display = DisplayStyle.None;
        loginScreen.SetEnabled(false);
        HideStartMenu(false);
    }

    private void OnCloseCreateUserButtonClicked()
    {
        createUserScreen.style.display = DisplayStyle.None;
        createUserScreen.SetEnabled(false);
        HideStartMenu(false);
    }

    private void OnCloseAchievementButtonClicked()
    {
        HideVisualElement(achievementScreen, true);
        HideStartMenu(false);
    }

    private void OnCloseHighScoreButtonClicked()
    {
        HideVisualElement(highscoreScreen, true);
        HideStartMenu(false);
    }

    private void OnCreateUserButtonClicked()
    {
        HideStartMenu(true);
        foreach (VisualElement screen in screens)
        {
            if (screen != createUserScreen && screen.enabledInHierarchy)
            {
                HideVisualElement(screen, true);
            }
            else if (screen == createUserScreen && screen.enabledInHierarchy == false)
            {
                HideVisualElement(screen, false);
            }
        }
    }

    public void OnLoginButtonClicked()
    {
        HideStartMenu(true);
        foreach (VisualElement screen in screens)
        {
            if (screen != loginScreen && screen.enabledInHierarchy)
            {
                HideVisualElement(screen, true);
            }
            else if (screen == loginScreen && screen.enabledInHierarchy == false)
            {
                HideVisualElement(screen, false);
            }
        }
    }

    private void OnLogOutButtonClicked()
    {
        if (WebManagerScript.ConnectionRunning)
        {
            if (WebManagerScript.CurrentUser != null)
            {
                DataTransfer_SO.Instance.resetEvent?.Invoke(); //Resets DataTransfer eg. Logs out among other stuff.
                logOutButton.style.display = DisplayStyle.None;
                logOutButton.SetEnabled(false);
            }
            else
            {
                Debug.Log("Failed to log out: No user logged in.");
            }
        }
        else { Debug.Log("Failed to log out: Connection to server is not running"); }
    }

    public void OnSendLoginButtonClicked()
    {
        if (WebManagerScript.ConnectionRunning)
        {
            if (WebManagerScript.CurrentUser == null)
            {
                WebManagerScript.RequestWithData<LoginDTO>(new LoginDTO(loginEmailTextField.value, loginPasswordTextField.value));
                Debug.Log($"Login in with: \nEmail: {loginEmailTextField.value} \nPassword: {loginPasswordTextField.value}");
            }
            else
            {
                Debug.Log("A user is already logged in");
            }
        }
        else
        {
            Debug.Log("Couldn't send login request: The connection wasn't running");
        }
        loginScreen.style.display = DisplayStyle.None;
        loginScreen.SetEnabled(false);
        HideStartMenu(false);

        //TODO: Validate input
    }

    public void OnSendCreateUserButtonClicked()
    {
        if (WebManagerScript.ConnectionRunning)
        {
            if (WebManagerScript.CurrentUser == null)
            {
                WebManagerScript.RequestWithData<CreateUserDTO>(new CreateUserDTO(createUserUserNameTextField.value, createUserEmailTextField.value, createUserPasswordTextField.value));
                Debug.Log($"Created user with: \nEmail: {createUserEmailTextField.value}\nUsername: {createUserUserNameTextField.value}\nPassword: {createUserPasswordTextField.value}");
            }
            else
            {
                Debug.Log("A user is already logged in");
            }
        }
        else
        {
            Debug.Log("Couldn't send user creation request: Connection wasn't running");
        }

        createUserScreen.style.display = DisplayStyle.None;
        createUserScreen.SetEnabled(false);
        HideStartMenu(false);
        //TODO: Validate Input
    }

    private void OnShowHighscoreButtonClicked()
    {
        HideStartMenu(true);
        foreach (VisualElement screen in screens)
        {
            if (screen != highscoreScreen && screen.enabledInHierarchy)
            {
                HideVisualElement(screen, true);
            }
            else if (screen == highscoreScreen && screen.enabledInHierarchy == false)
            {
                HideVisualElement(screen, false);
            }
        }

        //UPDATE highscoreContentLabel to show highscore

        WebManagerScript.Request = WebRequest.ShowHighscores;
        WebManagerScript.ChatActive = true;
        WebManagerScript.ShowHighscore = true;
        showOwn = false;

    }

    private void OnSwitchToHighScoreButtonClicked()
    {
        HideVisualElement(achievementScreen, true);
        HideVisualElement(highscoreScreen, false);

        //UPDATE highscoreContentLabel to show highscore

        WebManagerScript.Request = WebRequest.ShowHighscores;
        WebManagerScript.ChatActive = true;
        WebManagerScript.ShowHighscore = true;
        showOwn = false;

    }

    private void OnSwitchToAchievementButtonClicked()
    {
        HideVisualElement(highscoreScreen, true);
        HideVisualElement(achievementScreen, false);
        //UPDATE achievementContentLabel to show achievements

        WebManagerScript.Request = WebRequest.ShowAchievements;
        WebManagerScript.ChatActive = true;
        WebManagerScript.ShowAchievement = true;
        showOwn = false;

    }

    /// <summary>
    /// Sends score and achievements to server
    /// </summary>
    private void PublishHighscoreAndAchievements()
    {

        if (!WebManagerScript.ConnectionRunning || WebManagerScript.CurrentUser == null || !updatedScores)
        {

            Debug.LogWarning("No new achivements to send");
            return;

        }

        updatedScores = false;

        WebManagerScript.RequestWithData(new HighScoreDTO(WebManagerScript.CurrentUser.Name, WebManagerScript.CurrentUser.Email, DataTransfer_SO.Instance.RoundScore));
        WebManagerScript.RequestWithData(DataTransfer_SO.Instance.EarnedAchievements);

    }

    /// <summary>
    /// Sets own score
    /// </summary>
    /// <param name="score">Score transmitted via event</param>
    private void SetScoreLabel(HighScoreDTO score)
    {

        ownScore = score.Score;

    }

    /// <summary>
    /// Shows leaderboard
    /// </summary>
    /// <param name="scores">List of 10 highscores transmitted via event</param>
    private void SetLeaderboardField(List<HighScoreDTO> scores)
    {

        string text = string.Empty;

        foreach (HighScoreDTO score in scores)
            text += $"{score.UserName}: {score.Score}\n";

        highscoreContentLabel.text = text;

    }

    /// <summary>
    /// Shows 10 last earned achievements
    /// </summary>
    /// <param name="achievements">Shows 10 last earned achievements transmitted via event</param>
    private void SetAchievementsField(List<AchievementDTO> achievements)
    {

        string text = string.Empty;

        foreach (var achievement in achievements)
            text += $"{achievement.UserName} optjente {(Achievements)achievement.AchievementID}\n";

        achievementContentLabel.text = text;

    }

    /// <summary>
    /// Records own achievements
    /// </summary>
    /// <param name="achievements">Own achievements transmitted via event</param>
    private void ShowOwnAchievements(List<AchievementDTO> achievements)
    {

        ownAchievements = achievements;

    }

    /// <summary>
    /// Resets local cache of data
    /// </summary>
    private void ResetLocalData()
    {

        ownAchievements = null;
        ownScore = default;

    }

    /// <summary>
    /// Toggles between show own and global stats
    /// </summary>
    private void ShowOwnStatsToggle()
    {

        if (WebManagerScript.CurrentUser == null) return;

        showOwn = !showOwn;
        WebManagerScript.ChatActive = !showOwn;

        if (!showOwn && achievementScreen.enabledInHierarchy)
        {

            WebManagerScript.ShowAchievement = true;
            WebManagerScript.Request = WebRequest.ShowAchievements;

        }
        else if (!showOwn && highscoreScreen.enabledInHierarchy)
        {

            WebManagerScript.ShowHighscore = true;
            WebManagerScript.Request = WebRequest.ShowHighscores;

        }

        if (showOwn)
        {

            WebManagerScript.ShowAchievement = !showOwn;
            WebManagerScript.ShowHighscore = !showOwn;
            WebManagerScript.Request = WebRequest.OwnHighscore;
            WebManagerScript.Request = WebRequest.OwnAchievements;

        }

    }
    #endregion
}
