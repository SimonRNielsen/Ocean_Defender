using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TimeoutWarning : MonoBehaviour
{

    private Label textField;
    private VisualElement background;
    private float countDown;
    private bool backgroundSet = false;
    private readonly string startMenu = "StartMenu";
    [SerializeField] private Sprite backgroundSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    /// <summary>
    /// Resets and applies startup logic
    /// </summary>
    private void OnEnable()
    {

        if (textField == null)
        {

            var root = GetComponent<UIDocument>().rootVisualElement;

            if (root != null)
            {

                textField = root.Q<Label>("Timer");
                background = root.Q<VisualElement>("Window");

            }

        }

        if (!backgroundSet && backgroundSprite != null && background != null)
        {

            backgroundSet = true;
            background.style.backgroundImage = new StyleBackground(backgroundSprite);

        }

        countDown = 10f;

    }

    // Update is called once per frame
    void Update()
    {

        countDown -= Time.unscaledDeltaTime;

        if (countDown >= 0f)
            textField.text = $"No input detected, returning to main menu in: {(int)countDown + 1} s\nClick anywhere to cancel";
        else
            ReturnToStartMenu();

    }

    /// <summary>
    /// Closes all other scenes and loads start menu
    /// </summary>
    private void ReturnToStartMenu() => SceneManager.LoadScene(startMenu, LoadSceneMode.Single);

}
