using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class ScoreCounterUIBinding : MonoBehaviour
{
    public UIDocument uiDocument;
    public string scoreLabelName = "ScoreLabel";

    private Label scoreLabel;

    void Awake()
    {
        var root = uiDocument.rootVisualElement;
        scoreLabel = root.Q<Label>(scoreLabelName);
    }

    // DENNE skal være public og præcis denne signatur
    public void OnScoreChanged(int score, LocalizedString unit, int amount)
    {
        unit.StringChanged += (localized) =>
        {
            if (scoreLabel != null)
                scoreLabel.text = $"{score} {localized}";
        };
        unit.RefreshString();
    }
}
