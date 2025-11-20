using UnityEngine;
using UnityEngine.Events;

public class ScoreCounterScript : MonoBehaviour
{
    #region Fields
    [SerializeField, Tooltip("The current score")] private int score;
    [SerializeField, Tooltip("The name of the unit the score is counted in")] string scoreUnit;
    public UnityEvent<int, string, int> ScoreChanged;
    #endregion

    #region Methods

    /// <summary>
    /// Adds the given number to the score of the ScoreCounter
    /// </summary>
    /// <param name="amount">The amount that should be added to the score.</param>
    public void AddToScore(int amount)
    {
        this.score += amount;
        ScoreChanged.Invoke(score, scoreUnit, amount);
    }

    /// <summary>
    /// Subscribes to getScore event
    /// </summary>
    private void OnEnable()
    {

        DataTransfer_SO.Instance.getScore += SetRoundScore;

    }

    /// <summary>
    /// Unsubscribes to getScore event
    /// </summary>
    private void OnDisable()
    {

        DataTransfer_SO.Instance.getScore -= SetRoundScore;

    }

    /// <summary>
    /// Sets score on scriptable object "DataTransfer"
    /// </summary>
    private void SetRoundScore() => DataTransfer_SO.Instance.RoundScore = score;

    #endregion
}
