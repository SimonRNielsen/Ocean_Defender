using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using static SelectLanguage;

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



    //[Serializable]
    //public class Entry
    //{
    //    public TextKey key;
    //    [TextArea] public string danish;
    //    [TextArea] public string english;
    //    [TextArea] public string german;
    //}

    //public List<Entry> entries = new List<Entry>();

    //private Dictionary<TextKey, Entry> dict;


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
    //[Serializable]
    //public class LanguageStringArray
    //{

    //    [Tooltip("Name of the field or button")] public string tag;
    //    [SerializeField, Tooltip("Danish version")] private string danish;
    //    [SerializeField, Tooltip("English version")] private string english;
    //    [SerializeField, Tooltip("German version")] private string german;


    //    public string[] Strings
    //    {

    //        get
    //        {

    //            return new string[] { danish, english, german };

    //        }

    //    }
    //}

//    public void Init()
//    {
//        dict = new Dictionary<TextKey, Entry>();
//        foreach (var entry in entries)
//            dict[entry.key] = entry;
//    }

//    public string Get(TextKey key, Language lang)
//    {
//        if (dict == null)
//            Init();

//        if (!dict.TryGetValue(key, out var entry))
//            return $"MISSING:{key}";

//        return lang switch
//        {
//            Language.Danish => entry.danish,
//            Language.English => entry.english,
//            Language.German => entry.german,
//            _ => "MISSING_LANG"
//        };
//    }

//    public enum Language
//    {
//        Danish,
//        English,
//        German
//    }
//}
//#endregion
//#endregion



//public class LanguageManager : MonoBehaviour
//{
//    public static LanguageManager Instance;
//    public DataTransfer_SO data;
//    public DataTransfer_SO.Language currentLanguage;

//    public event Action OnLanguageChanged;

//    private void Awake()
//    {
//        Instance = this;
//        data.Init();
//    }

//    public string Get(TextKey key)
//    {
//        return data.Get(key, currentLanguage);
//    }

//    public void SetLanguage(DataTransfer_SO.Language lang)
//    {
//        currentLanguage = lang;

//        // tell everybody to update
//        OnLanguageChanged?.Invoke();
//    }


}
#endregion
#endregion

