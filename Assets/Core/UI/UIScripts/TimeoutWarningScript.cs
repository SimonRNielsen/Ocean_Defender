using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimeoutWarning : MonoBehaviour
{

    private Label textField;
    private VisualElement background;
    private float countDown;
    private bool backgroundSet = false, returningToStart = false;
    private readonly string startMenu = "StartMenu";
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private float countDownTime = 10f;
    [SerializeField] private Color backgroundColor;
    [SerializeField, Range(0, 1)] private float backgroundOpaque = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    /// <summary>
    /// Resets and applies startup logic
    /// </summary>
    private void OnEnable()
    {

        returningToStart = false;

        Time.timeScale = 0f;

        countDown = countDownTime;

        if (textField == null)
        {

            var root = GetComponent<UIDocument>().rootVisualElement;

            if (root != null)
            {

                textField = root.Q<Label>("Timer");
                background = root.Q<VisualElement>("Window");

            }

        }

        if (!backgroundSet && background != null)
        {

            backgroundSet = true;

            if (backgroundSprite != null)
                background.style.backgroundImage = new StyleBackground(backgroundSprite);
            else
            {

                backgroundColor.a = backgroundOpaque;
                background.style.backgroundColor = backgroundColor;

            }

        }

    }


    private void OnDisable()
    {

        if (!returningToStart && !RoundCountDownTimerScript.RoundOver)
            Time.timeScale = 1f;

        TimingsScript.WarningActive = false;

    }

    // Update is called once per frame
    void Update()
    {

        countDown -= Time.unscaledDeltaTime;

        if (countDown >= 0f)
            textField.text = $"Returnerer til start om:\nReturning to start in:\nRückkehr zum Startpunkt in:\n{(int)countDown + 1} s\nClick annullerer/aborts/storniert";
        else if (!returningToStart)
            ReturnToStartMenu();

    }

    /// <summary>
    /// Closes all other scenes and loads start menu
    /// </summary>
    private void ReturnToStartMenu()
    {

        if (returningToStart) return;

        returningToStart = true;
        DataTransfer_SO.Instance.resetEvent?.Invoke();

        SceneManager.LoadScene(startMenu, LoadSceneMode.Single);

    }

}
