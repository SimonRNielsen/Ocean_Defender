using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class ScoreCounterPlantUIBinding : MonoBehaviour
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
        string unitText = unit.GetLocalizedString();
        scoreLabel.text = score + " " + unitText;
    }
}
