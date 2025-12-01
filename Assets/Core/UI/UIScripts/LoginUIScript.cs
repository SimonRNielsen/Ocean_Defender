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
    private Button logOutButton;

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

    private bool loggedIn = false;
    #endregion


    #region Methods

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        //Buttons to upon screens
        loginButton = root.Q<Button>("LoginButton");
        createUserButton = root.Q<Button>("CreateUserButton");
        logOutButton = root.Q<Button>("LogOutButton");

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
        logOutButton.clicked += OnLogOutButtonClicked;
        sendLoginButton.clicked += OnSendLoginButtonClicked;
        closeLoginButton.clicked += OnCloseLoginButtonClicked;
        sendCreateUserButton.clicked += OnSendCreateUserButtonClicked;
        closeCreateUserButton.clicked += OnCloseCreateUserButtonClicked;

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
            if(logOutButton.enabledInHierarchy == false)
            {
                logOutButton.SetEnabled(true);
                logOutButton.style.display= DisplayStyle.Flex;
            }
        }
        else
        {
            if(loginButton.enabledInHierarchy == false)
            {
                loginButton.style.display= DisplayStyle.Flex;
                loginButton.SetEnabled(true);
            }
            if(logOutButton.enabledInHierarchy == true)
            {
                logOutButton.style.display = DisplayStyle.None;
                logOutButton.SetEnabled(false);
            }
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
    #endregion
}
