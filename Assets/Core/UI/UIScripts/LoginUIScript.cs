using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginUIScript : MonoBehaviour
{
    #region Fields
    private VisualElement root;

    //Buttons for openeing screens
    private Button loginButton;
    private Button createUserButton;

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

    #endregion


    #region Methods

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        //Buttons to upon screens
        loginButton = root.Q<Button>("LoginButton");
        createUserButton = root.Q<Button>("CreateUserButton");

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
    }

    private void OnEnable()
    {
        loginButton.clicked += OnLoginButtonClicked;
        createUserButton.clicked += OnCreateUserButtonClicked;
        sendLoginButton.clicked += OnSendLoginButtonClicked;
        closeLoginButton.clicked += OnCloseLoginButtonClicked;
        sendCreateUserButton.clicked += OnSendCreateUserButtonClicked;
        closeCreateUserButton.clicked += OnCloseCreateUserButtonClicked;

    }


    private void OnDisable()
    {
        loginButton.clicked -= OnLoginButtonClicked;
        createUserButton.clicked -= OnCreateUserButtonClicked;
        sendLoginButton.clicked -= OnSendLoginButtonClicked;
        sendCreateUserButton.clicked -= OnSendCreateUserButtonClicked;
        closeCreateUserButton.clicked -= OnCloseCreateUserButtonClicked;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    private void OnCreateUserButtonClicked()
    {
        if (loginScreen.enabledInHierarchy)
        {
            loginScreen.style.display = DisplayStyle.None;
            loginScreen.SetEnabled(false);
        }
        HideStartMenu(true);
        createUserScreen.SetEnabled(true);
        createUserScreen.style.display = DisplayStyle.Flex;
    }

    public void OnLoginButtonClicked()
    {
        if (createUserScreen.enabledInHierarchy)
        {
            createUserScreen.style.display = DisplayStyle.None;
            createUserScreen.SetEnabled(false);
        }
        HideStartMenu(true);
        loginScreen.SetEnabled(true);
        loginScreen.style.display = DisplayStyle.Flex;
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


    private void PublishHighscoreAndAchievements()
    {

        if (!WebManagerScript.ConnectionRunning || WebManagerScript.CurrentUser == null) return;

        WebManagerScript.RequestWithData(new HighScoreDTO(WebManagerScript.CurrentUser.Name, WebManagerScript.CurrentUser.Email, DataTransfer_SO.Instance.RoundScore));
        WebManagerScript.RequestWithData(DataTransfer_SO.Instance.EarnedAchievements);

    }
    #endregion
}
