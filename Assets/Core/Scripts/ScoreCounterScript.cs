using UnityEngine;
using UnityEngine.Events;

public class ScoreCounterScript : MonoBehaviour
{
    #region Fields
    [SerializeField, Tooltip("The current score")] private int score;
    [SerializeField, Tooltip("The name of the unit the score is counted in")] string scoreUnit;
    public UnityEvent<int, string, int> ScoreChanged;

    [SerializeField, Tooltip("The achievement for the level")]
    public GameObject achievement;
    [SerializeField, Tooltip("The amoungt of point to active the achievement")]
    public int achievementScore;
    private bool achievementUnloocked = false;

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

        //Spawning the achievement when achievementScore reached
        if (score >= achievementScore)
        {
            achievement.SetActive(true);
            achievementUnloocked = true;
        }
        else if (achievementUnloocked == true)
        {
            achievement.transform.position = new Vector3(8, 2, 0);
            achievement.transform.localScale = Vector3.one / 4;
        }

        
    }

    #endregion
}
