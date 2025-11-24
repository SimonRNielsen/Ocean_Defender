using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;


public class TimingsScript : MonoBehaviour
{

    #region Fields

    private static float timeSinceLastCheck, lastActivityTime;
    private readonly float intervalBetweenChecks = 1f;
    private readonly string startMenu = "StartMenu", returnToMenuWarning = "TimeoutWarning";
    private static bool inputDetected = false, warningActive = false;
    [SerializeField, Range(5, 50)] private float inactivityTimeLimit = 5f;

    #endregion
    #region Properties

    /// <summary>
    /// Bool being set by RaycastScript to indicate input being detected
    /// </summary>
    public static bool InputDetected
    {

        get => inputDetected;
        set
        {

            if (value)
                lastActivityTime = Time.unscaledTime;

            inputDetected = value;

        }

    }


    /// <summary>
    /// Logic to see if warning is active
    /// </summary>
    public static bool WarningActive
    {

        get => warningActive;
        set => warningActive = value;

    }


    #endregion
    #region Methods

    /// <summary>
    /// Initiation of script, ensures it's not destroyed
    /// </summary>
    private void Awake()
    {

        DontDestroyOnLoad(gameObject);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    /// <summary>
    /// Resets handling logic
    /// </summary>
    private void OnEnable()
    {

        timeSinceLastCheck = Time.unscaledTime;
        lastActivityTime = Time.unscaledTime;

    }

    /// <summary>
    /// Runs checks for activity or startmenu at intervals
    /// </summary>
    void Update()
    {

        if (Time.unscaledTime - timeSinceLastCheck >= intervalBetweenChecks)
        {

            timeSinceLastCheck = Time.unscaledTime;

            if (!IsMenuOrWarningActive())
            {

                if (Time.unscaledTime - lastActivityTime >= inactivityTimeLimit)
                {

                    lastActivityTime = Time.unscaledTime;
                    if (!warningActive)
                        StartCoroutine(LoadWarning());

                }

            }
            else
                lastActivityTime = Time.unscaledTime;

        }

        if (inputDetected && warningActive)
            StartCoroutine(UnloadWarning());

        inputDetected = false;

    }

    /// <summary>
    /// Checks for startmenu (and return-to-menu warning)
    /// </summary>
    /// <returns>True if startmenu is active</returns>
    private bool IsMenuOrWarningActive() => SceneManager.GetSceneByName(startMenu).isLoaded || SceneManager.GetSceneByName(returnToMenuWarning).isLoaded;

    /// <summary>
    /// Loads warning screen
    /// </summary>
    /// <returns>Loading of scene</returns>
    private IEnumerator LoadWarning()
    {

        if (SceneManager.GetSceneByName(returnToMenuWarning).isLoaded)
        {

            warningActive = true;
            yield break;

        }

        yield return SceneManager.LoadSceneAsync(returnToMenuWarning, LoadSceneMode.Additive);

        warningActive = true;

    }

    /// <summary>
    /// Unloads warning screen
    /// </summary>
    /// <returns>Unloading of scene</returns>
    private IEnumerator UnloadWarning()
    {

        if (!SceneManager.GetSceneByName(returnToMenuWarning).isLoaded)
            yield break;

        yield return SceneManager.UnloadSceneAsync(returnToMenuWarning);

        warningActive = false;

    }

    #endregion

}
