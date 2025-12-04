using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class ScoreCounterScript : MonoBehaviour
{
    #region Fields
    [SerializeField, Tooltip("The current score")] private int score;
    [SerializeField, Tooltip("The name of the unit the score is counted in")] LocalizedString scoreUnit;
    public UnityEvent<int, LocalizedString, int> ScoreChanged;

    [SerializeField, Tooltip("The achievement for the level")]
    public GameObject achievement;
    [SerializeField, Tooltip("The amoungt of point to active the achievement")]
    public int achievementScore;

    private int quizScore;
    [SerializeField, Tooltip("The quiz achievement")]
    public GameObject achievementQuiz;
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
        if (score >= achievementScore && achievement.activeSelf == false)
        {
            achievement.SetActive(true);
        }
        else if (achievement.activeSelf == true)
        {
            achievement.transform.position = new Vector3(8, 2, 0);
            achievement.transform.localScale = Vector3.one / 4;
        }

        

    }

    private void Update()
    {
        quizScore = DataTransfer_SO.Instance.QuizScore;
        if (quizScore > 3)
        {
            achievementQuiz.SetActive(true);
        }
        
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
