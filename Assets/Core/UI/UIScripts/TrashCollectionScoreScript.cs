using UnityEngine;
using UnityEngine.UIElements;

public class TrashCollectionScoreScript : MonoBehaviour
{
    private Label scoreLabel;
    [SerializeField, Tooltip("The default text, the UI should show when there is no score.")] private string defaultLabelText;
    [SerializeField, Tooltip("The goal amoint of the score")] private int goalScore;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreLabel");
        scoreLabel.text = defaultLabelText;
    }


    /// <summary>
    /// Updates the scorelabel with the score given
    /// </summary>
    /// <param name="score">The score that the scorelabel should show</param>
    /// <param name="unit">The unit that the score is in</param>
    public void OnScoreChanged(int score, string unit)
    {
        scoreLabel.text = score + " / " + goalScore + " " + unit;
    }
}
