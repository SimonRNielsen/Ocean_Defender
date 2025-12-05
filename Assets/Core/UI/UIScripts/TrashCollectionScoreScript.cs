using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class TrashCollectionScoreScript : MonoBehaviour
{
    private Label scoreLabel;
    private Label scoreAmountLabel;
    //[SerializeField, Tooltip("The default text, the UI should show when there is no score.")] private string defaultLabelText;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreLabel");
        scoreAmountLabel = root.Q<Label>("ScoreAmountLabel");
        //scoreLabel.text = defaultLabelText;
        scoreAmountLabel.text = "";
        scoreAmountLabel.style.display = DisplayStyle.None;

    }


    /// <summary>
    /// Updates the scorelabel with the score given
    /// </summary>
    /// <param name="score">The score that the scorelabel should show</param>
    /// <param name="unit">The unit that the score is in</param>
    public void OnScoreChanged(int score, LocalizedString unit, int amount)
    {
        scoreLabel.text = score + " " + unit;
        StartCoroutine(ShowScoreAmount(amount));
    }
    /// <summary>
    /// Displays the amount the player socred on the screen int the scoreAmountLabel
    /// </summary>
    /// <param name="amount"></param>
    public IEnumerator ShowScoreAmount(int amount)
    {
        scoreAmountLabel.text = ("+" + amount);
        //scoreAmountLabel.SetEnabled(true);
        scoreAmountLabel.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(1f);
        //scoreAmountLabel.SetEnabled(false);
        scoreAmountLabel.style.display = DisplayStyle.None;
        scoreAmountLabel.text = "";
    }

 
}
