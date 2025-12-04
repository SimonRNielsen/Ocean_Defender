//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.UIElements;

//public class ScoreCounterPlantUIBinding : MonoBehaviour
//{
//    [SerializeField] private ScoreCounterScript scoreCounter;
//    [SerializeField] private UIDocument uiDocument;
//    [SerializeField] private LocalizedString scoreLocalizedText;

//    private Label label;

//    private void Awake()
//    {
//        var root = uiDocument.rootVisualElement;
//        label = root.Q<Label>("ScoreLabel");
//    }

//    private void OnEnable()
//    {
//        if (scoreCounter != null)
//            //scoreCounter.ScoreChanged += OnScoreChanged;

//        // Kun én gang!
//        scoreLocalizedText.StringChanged += UpdateLabel;
//    }

//    private void OnDisable()
//    {
//        if (scoreCounter != null)
//            //scoreCounter.ScoreChanged -= OnScoreChanged;

//        // Fjern event korrekt
//        scoreLocalizedText.StringChanged -= UpdateLabel;
//    }

//    private void OnScoreChanged(int score, LocalizedString unit, int amount)
//    {
//        // Opdater argument til lokaliseret tekst
//        scoreLocalizedText.Arguments = new object[] { score };

//        // Trigger opdatering
//        scoreLocalizedText.RefreshString();
//    }

//    private void UpdateLabel(string value)
//    {
//        if (label != null)
//            label.text = value;
//    }
//}

//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.UIElements;

//public class ScoreCounterPlantUIBinding : MonoBehaviour
//{
//    [SerializeField] private UIDocument uiDocument;
//    [SerializeField] private LocalizedString scoreLocalizedText;

//    private Label label;

//    private void Awake()
//    {
//        var root = uiDocument.rootVisualElement;
//        label = root.Q<Label>("ScoreLabel");
//    }

//    // Denne metode kaldes af UnityEvent i INSPECTOR
//    public void OnScoreChanged(int score, LocalizedString unit, int amount)
//    {
//        scoreLocalizedText.Arguments = new object[] { score };

//        scoreLocalizedText.StringChanged += UpdateLabel;
//        scoreLocalizedText.RefreshString();
//    }

//    private void UpdateLabel(string value)
//    {
//        if (label != null)
//            label.text = value;
//    }
//}





using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class ScoreCounterPlantUIBinding : MonoBehaviour
{
    [SerializeField] private ScoreCounterScript scoreCounter;
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private LocalizedString scoreLocalizedText;

    private Label label;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;
        label = root.Q<Label>("ScoreLabel");
    }

    private void OnEnable()
    {
        if (scoreCounter != null)
            scoreCounter.ScoreChanged.AddListener(OnScoreChanged);
    }

    private void OnDisable()
    {
        if (scoreCounter != null)
            scoreCounter.ScoreChanged.RemoveListener(OnScoreChanged);
    }

    // --- Denne metode kaldes af ScoreCounter ---
    public void OnScoreChanged(int score, LocalizedString unit, int amount)
    {
        scoreLocalizedText.Arguments = new object[] { score };

        // Fjern tidligere binding for at undgå dobbelt subscriptions
        scoreLocalizedText.StringChanged -= UpdateLabel;
        scoreLocalizedText.StringChanged += UpdateLabel;

        scoreLocalizedText.RefreshString();
    }

    private void UpdateLabel(string value)
    {
        if (label != null)
            label.text = value;
    }
}
