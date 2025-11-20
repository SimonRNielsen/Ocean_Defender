using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RoundCountDownTimerScript : MonoBehaviour
{

    [SerializeField, Range(10, 180), Header("Timer")] private int roundTime = 120;
    [SerializeField, Range(0, 1), Space, Header("Styling")] private float opaqueness = 1f;
    [SerializeField] private Color barColor, textColor, backgroundColor;
    [SerializeField] private bool barOpaque = false, textOpaque = false, backgroundOpaque = false;
    [SerializeField, Range(0, 10)] private int margin = 3;
    [SerializeField, Space, Header("Placement and displaymode")] private VerticalAlignment verticalPlacement = VerticalAlignment.Top;
    [SerializeField] private HorizontalAlignment horizontalPlacement = HorizontalAlignment.Right;
    [SerializeField] private bool turnBarIntoLabel = false, displayText = true;
    [SerializeField] private string[] returnScenes;
    private readonly float notOpaque = 1f;
    private ProgressBar progressBar;
    private Label label;
    private VisualElement placement;
    private float roundTimeRemaining;
    private bool roundEnded;


    private float RoundTimeRemaining { get => roundTimeRemaining + 1; set => UpdateTimer(value); }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }


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


    private void OnEnable()
    {

        roundEnded = false;
        RoundTimeRemaining = (float)roundTime;

    }

    // Update is called once per frame
    void Update()
    {

        RoundTimeRemaining -= Time.deltaTime;

    }


    private bool DeterminePlacement()
    {

        string placementID = string.Empty;

        switch (verticalPlacement)
        {
            case VerticalAlignment.Top:
                switch (horizontalPlacement)
                {
                    case HorizontalAlignment.Left:
                        placementID = "TopLeft";
                        break;
                    case HorizontalAlignment.Center:
                        placementID = "TopCentre";
                        break;
                    case HorizontalAlignment.Right:
                        placementID = "TopRight";
                        break;
                }
                break;
            case VerticalAlignment.Middle:
                switch (horizontalPlacement)
                {
                    case HorizontalAlignment.Left:
                        placementID = "MiddleLeft";
                        break;
                    case HorizontalAlignment.Center:
                        placementID = "MiddleCentre";
                        break;
                    case HorizontalAlignment.Right:
                        placementID = "MiddleRight";
                        break;
                }
                break;
            case VerticalAlignment.Bottom:
                switch (horizontalPlacement)
                {
                    case HorizontalAlignment.Left:
                        placementID = "BottomLeft";
                        break;
                    case HorizontalAlignment.Center:
                        placementID = "BottomCentre";
                        break;
                    case HorizontalAlignment.Right:
                        placementID = "BottomRight";
                        break;
                }
                break;
        }

        var root = GetComponent<UIDocument>().rootVisualElement;
        placement = root.Q<VisualElement>(placementID);

        if (placement != null)
            return true;

        return false;

    }


    private void CreateTimer()
    {

        if (!turnBarIntoLabel)
        {

            progressBar = new ProgressBar();



            placement.Add(progressBar);

        }
        else
        {

            label = new Label();



            placement.Add(label);

        }

    }


    private void UpdateTimer(float time)
    {

        roundTimeRemaining = time;

        if (roundTimeRemaining <= 0f)
        {

            if (!roundEnded)
            {

                roundEnded = true;
                StartCoroutine(EndRound(returnScenes));

            }
            return;

        }

        if (label == null && progressBar == null)
        {

            Debug.LogWarning("No displaymethod found");
            return;

        }

        int displayedTime = (int)RoundTimeRemaining;
        string text = displayText ? $"Time Remaining: {displayedTime}s" : string.Empty;

        if (!turnBarIntoLabel)
        {



        }
        else
        {



        }

    }


    private IEnumerator EndRound(string[] sceneNames)
    {

        for (int i = 0; i < sceneNames.Length; i++)
        {

            if (i == 0)
                yield return SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Single);
            else
                yield return SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive);

        }

    }

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
