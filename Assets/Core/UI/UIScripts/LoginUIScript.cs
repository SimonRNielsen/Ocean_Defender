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
        highscoreContentLabel = root.Q<Label>("HighscoreContentLabel");
        highscoreScreen = root.Q<VisualElement>("HighscoreScreen");

        //Achievement elements:
        closeAchievementButton = root.Q<Button>("CloseAchievementButton");
        switchToHighscoreButton = root.Q<Button>("SwitchToHighscoreButton");
        achievementContentLabel = root.Q<Label>("AchievementContentLabel");
        achievementScreen = root.Q<VisualElement>("AchievementScreen");

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
    }

    private void OnSwitchToHighScoreButtonClicked()
    {
        HideVisualElement(achievementScreen, true);
        HideVisualElement(highscoreScreen, false);

        //UPDATE highscoreContentLabel to show highscore

    }

    private void OnSwitchToAchievementButtonClicked()
    {
        HideVisualElement(highscoreScreen, true);
        HideVisualElement(achievementScreen, false);
        //UPDATE achievementContentLabel to show achievements
    }


    private void PublishHighscoreAndAchievements()
    {

        if (!WebManagerScript.ConnectionRunning || WebManagerScript.CurrentUser == null) return;

        WebManagerScript.RequestWithData(new HighScoreDTO(WebManagerScript.CurrentUser.Name, WebManagerScript.CurrentUser.Email, DataTransfer_SO.Instance.RoundScore));
        WebManagerScript.RequestWithData(DataTransfer_SO.Instance.EarnedAchievements);

    }
    #endregion
}
