using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTransfer_SO", menuName = "Scriptable Objects/DataTransfer_SO")]
public class DataTransfer_SO : ScriptableObject
{

    #region Singleton

    private static DataTransfer_SO instance;

    public static DataTransfer_SO Instance
    {

        get
        {

            if (instance == null)
                instance = Resources.Load<DataTransfer_SO>("DataTransfer_SO");

            return instance;

        }

    }

    #endregion
    #region Events

    /// <summary>
    /// Plays sound of top of other sounds
    /// </summary>
    public Action<AudioClip> oneShotSoundEvent;
    /// <summary>
    /// Plays sound repeatedly
    /// </summary>
    public Action<AudioClip> loopingSoundEvent;
    /// <summary>
    /// Plays sound once
    /// </summary>
    public Action<AudioClip> playSoundEvent;
    /// <summary>
    /// Reset logic event
    /// </summary>
    public Action resetEvent;
    /// <summary>
    /// Event to set "RoundScore"
    /// </summary>
    public Action getScore;
    /// <summary>
    /// Event to transmit a list of highscores (leaderboard) to UI
    /// </summary>
    public Action<List<HighScoreDTO>> transmitScores;
    /// <summary>
    /// Event to transmit own score to UI
    /// </summary>
    public Action<HighScoreDTO> transmitScore;
    /// <summary>
    /// Event to transmit latest earned achievements to UI
    /// </summary>
    public Action<List<AchievementDTO>> transmitAchievements;
    /// <summary>
    /// Event to transmit all achievements earned by logged in user to UI
    /// </summary>
    public Action<List<AchievementDTO>> transmitOwnAchievements;

    #endregion
    #region Fields

    /// <summary>
    /// Indicates score achieved during recently completed round
    /// </summary>
    public int RoundScore;

    public List<AchievementDTO> EarnedAchievements;


    public float roundTimeRemaining;

    Dictionary<Enum, string> dansk = new Dictionary<Enum, string>();
    Dictionary<Enum, string> tysk = new Dictionary<Enum, string>();
    Dictionary<Enum, string> engelsk = new Dictionary<Enum, string>();
    Dictionary<Enum, string> selectedLanguage = new Dictionary<Enum, string>();

    #endregion
    #region Methods

    /// <summary>
    /// Resets events to avoid memory leaks
    /// </summary>
    private void OnEnable()
    {
        oneShotSoundEvent = null;
        loopingSoundEvent = null;
        playSoundEvent = null;
        resetEvent = null;
        getScore = null;
        transmitAchievements = null;
        transmitOwnAchievements = null;
        transmitScore = null;
        transmitScores = null;

        EarnedAchievements = new List<AchievementDTO>();
        RoundScore = 0;
        resetEvent += ResetWhileRunning;

        roundTimeRemaining = 0;
    }

    /// <summary>
    /// Reset of score
    /// </summary>
    private void ResetWhileRunning()
    {

        RoundScore = 0;
        EarnedAchievements.Clear();

    }


    #region SPROG
    [Serializable]
    public class LanguageStringArray
    {

        [Tooltip("Name of the field or button")] public string tag;
        [SerializeField, Tooltip("Danish version")] private string danish;
        [SerializeField, Tooltip("English version")] private string english;
        [SerializeField, Tooltip("German version")] private string german;


        public string[] Strings
        {

            get
            {

                return new string[] { danish, english, german };

            }

        }

    }
    #endregion
    #endregion

}
