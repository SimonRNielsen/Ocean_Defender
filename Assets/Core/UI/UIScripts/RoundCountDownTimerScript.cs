using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RoundCountDownTimerScript : MonoBehaviour
{

    #region Fields

    [SerializeField, Range(10, 180), Header("Timer")] private int roundTime = 120;
    [SerializeField, Range(0, 1), Space, Header("Styling")] private float opaqueness = 1f;
    [SerializeField] private Color barColor, textColor, backgroundColor;
    [SerializeField] private bool barOpaque = false, textOpaque = false, backgroundOpaque = false;
    [SerializeField, Range(0, 100)] private int margin = 10;
    [SerializeField, Space, Header("Placement and displaymode")] private VerticalAlignment verticalPlacement = VerticalAlignment.Top;
    [SerializeField] private HorizontalAlignment horizontalPlacement = HorizontalAlignment.Right;
    [SerializeField] private bool turnBarIntoLabel = false, displayText = true;
    [SerializeField] private Sprite roundOverSprite;
    [SerializeField, Space, Header("Scenes to open on exit")] private string[] returnScenes;
    private readonly float notOpaque = 1f;
    private readonly string roundOver = "RoundOver", buttonName = "OKButton", scoreLabelName = "ScoreLabel";
    private ProgressBar progressBar;
    private Label label, scoreLabel;
    private VisualElement placement, centre;
    private Button button;
    private float roundTimeRemaining;
    private bool roundEnded, scoreSet;


    #endregion
    #region Properties

    /// <summary>
    /// Time remaining of round
    /// </summary>
    private float RoundTimeRemaining { get => roundTimeRemaining; set => UpdateTimer(value); }

    /// <summary>
    /// Bool to indicate that time has run out
    /// </summary>
    private bool RoundEnded
    {

        get => roundEnded;
        set
        {

            if (!value)
                RoundTimeRemaining = (float)roundTime;

            roundEnded = value;

        }

    }

    /// <summary>
    /// Bool for other mechanics to check if round is over
    /// </summary>
    public static bool RoundOver { get; private set; }

    #endregion
    #region Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    /// <summary>
    /// Builds UI and sets values and properties according to settings
    /// </summary>
    private void Awake()
    {

        barColor.a = barOpaque ? opaqueness : notOpaque;
        textColor.a = textOpaque ? opaqueness : notOpaque;
        backgroundColor.a = backgroundOpaque ? opaqueness : notOpaque;

        if (DeterminePlacement())
            CreateTimer();
        else
            Debug.LogWarning("Couldn't determine location for round-timer");

    }

    /// <summary>
    /// (Re)sets UI logic
    /// </summary>
    private void OnEnable()
    {

        RoundEnded = false;
        centre.visible = false;
        scoreSet = false;
        RoundOver = false;

    }

    /// <summary>
    /// Disables button action
    /// </summary>
    private void OnDisable()
    {

        button.clicked -= ButtonAction;

    }

    // Update is called once per frame
    void Update()
    {

        RoundTimeRemaining -= Time.deltaTime;

        DataTransfer_SO.Instance.roundTimeRemaining = roundTimeRemaining;
    }

    /// <summary>
    /// Determines where to put custom UI element
    /// </summary>
    /// <returns>True if operation was successful</returns>
    private bool DeterminePlacement()
    {

        string placementID = string.Empty;

        switch (verticalPlacement)
        {
            default:
            case VerticalAlignment.Top:
                placementID = "Top";
                break;
            case VerticalAlignment.Middle:
                placementID = "Middle";
                break;
            case VerticalAlignment.Bottom:
                placementID = "Bottom";
                break;
        }
        switch (horizontalPlacement)
        {
            case HorizontalAlignment.Left:
                placementID += "Left";
                break;
            case HorizontalAlignment.Center:
                placementID += "Centre";
                break;
            default:
            case HorizontalAlignment.Right:
                placementID += "Right";
                break;
        }

        var root = GetComponent<UIDocument>().rootVisualElement;
        placement = root.Q<VisualElement>(placementID);
        centre = root.Q<VisualElement>(roundOver);
        button = root.Q<Button>(buttonName);
        scoreLabel = root.Q<Label>(scoreLabelName);

        if (centre != null && roundOverSprite != null)
            centre.style.backgroundImage = new StyleBackground(roundOverSprite);

        if (placement != null && centre != null && button != null && scoreLabel != null)
            return true;
        else
            Debug.LogWarning("Not all UI-elements found");

        return false;

    }

    /// <summary>
    /// Creates UI elements based on settings
    /// </summary>
    private void CreateTimer()
    {

        if (!turnBarIntoLabel)
        {

            progressBar = new ProgressBar();

            progressBar.style.width = new Length(100, LengthUnit.Percent);
            progressBar.lowValue = 0;
            progressBar.highValue = roundTime;
            progressBar.style.color = textColor;
            /*progressBar.style.backgroundColor = backgroundColor; Doesn't access the right visual element. 
              See under USS Classes: https://docs.unity3d.com/6000.2/Documentation/Manual/UIE-uxml-element-ProgressBar.html  
             */
            progressBar.style.marginBottom = margin;
            progressBar.style.marginTop = margin;
            progressBar.style.marginLeft = margin;
            progressBar.style.marginRight = margin;

            placement.Add(progressBar);

            //Acceses the visualelement respsponsible for the styling of the progress bar process, and changes color.
            var progressBarVisualElement = placement.Q(className: "unity-progress-bar__progress");
            progressBarVisualElement.style.backgroundColor = barColor;

            //Acceses the visualelement respsponsible for the styling of the progress bar background, and changes color.
            var progressBarBackgroundVisualElement = placement.Q(className: "unity-progress-bar__background");
            progressBarBackgroundVisualElement.style.backgroundColor = backgroundColor;
        }
        else
        {

            label = new Label();

            label.style.color = textColor;
            label.style.backgroundColor = backgroundColor;

            label.style.marginBottom = margin;
            label.style.marginTop = margin;
            label.style.marginLeft = margin;
            label.style.marginRight = margin;

            placement.Add(label);

        }

    }

    /// <summary>
    /// Update logic
    /// </summary>
    /// <param name="time">Time to set</param>
    private void UpdateTimer(float time)
    {

        roundTimeRemaining = time;

        if (scoreSet) return;

        if (centre.visible)
        {

            if (!scoreSet)
            {

                int score = DataTransfer_SO.Instance.RoundScore;

                scoreLabel.text = $"{score}";
                if (score != 0)
                    scoreSet = true;


            }
            return;

        }

        if (roundTimeRemaining <= 0f && !centre.visible)
        {

            centre.visible = true;

            DataTransfer_SO.Instance.getScore?.Invoke();
            RoundOver = true;
            Time.timeScale = 0f;
            button.clicked += ButtonAction;

        }

        if (label == null && progressBar == null)
        {

            Debug.LogWarning("No displaymethod found");
            return;

        }

        int displayedTime = centre.visible ? 0 : (int)RoundTimeRemaining + 1;
        string text = displayText ? $"Time Remaining: {displayedTime}s" : string.Empty;

        if (!turnBarIntoLabel)
        {

            progressBar.title = text;
            progressBar.value = RoundTimeRemaining;

        }
        else
            label.text = text;

    }

    /// <summary>
    /// Handles loading/unloading of all scenes
    /// </summary>
    /// <param name="sceneNames">Array of scenes to load</param>
    /// <returns>Loading/unloading of scenes</returns>
    private IEnumerator EndRound(string[] sceneNames)
    {

        int activeSceneCount = SceneManager.sceneCount;
        List<string> sceneNamesToUnload = new List<string>();

        for (int i = 0; i < activeSceneCount; i++)
            sceneNamesToUnload.Add(SceneManager.GetSceneAt(i).name);

        for (int i = 0; i < sceneNamesToUnload.Count; i++)
            if (sceneNamesToUnload[i] != gameObject.scene.name)
                yield return SceneManager.UnloadSceneAsync(sceneNamesToUnload[i]);

        DataTransfer_SO.Instance.resetEvent?.Invoke();

        for (int i = 0; i < sceneNames.Length; i++)
            yield return SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive);

        yield return SceneManager.UnloadSceneAsync(gameObject.scene);

    }

    /// <summary>
    /// Action for when button is pressed
    /// </summary>
    private void ButtonAction()
    {

        if (!RoundEnded)
        {

            Time.timeScale = 1f;
            RoundEnded = true;
            StartCoroutine(EndRound(returnScenes));

        }

    }


    #endregion

}


public enum VerticalAlignment
{

    Top,
    Middle,
    Bottom

}


public enum HorizontalAlignment
{

    Left,
    Center,
    Right

}
