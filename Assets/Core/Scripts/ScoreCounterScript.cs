using UnityEngine;
using UnityEngine.Events;

public class ScoreCounterScript : MonoBehaviour
{
    #region Fields
    [SerializeField, Tooltip("The current score")] private int score;
    [SerializeField, Tooltip("The name of the unit the score is counted in")] string scoreUnit;
    public UnityEvent<int, string> ScoreChanged;
    #endregion

    #region Methods

    /// <summary>
    /// Adds the given number to the score of the ScoreCounter
    /// </summary>
    /// <param name="amount">The amount that should be added to the score.</param>
    public void AddToScore(int amount)
    {
        this.score += amount;
        ScoreChanged.Invoke(score, scoreUnit);
        Debug.Log($"New score: {score}{scoreUnit}");
    }

    #endregion
}
