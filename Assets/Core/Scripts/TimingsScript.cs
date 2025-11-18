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
    private static bool inputDetected = false;
    private bool warningActive = false;
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
    //public static bool warningActive { get; set; } = false;

    #endregion
    #region Methods

    /// <summary>
    /// Initiation of script, ensures it's not destroyed
    /// </summary>
    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
        timeSinceLastCheck = Time.unscaledTime;
        lastActivityTime = Time.unscaledTime;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    /// <summary>
    /// Resets handling logic
    /// </summary>
    private void OnEnable()
    {

        inputDetected = false;
        warningActive = false;

    }

    /// <summary>
    /// Runs checks for activity or startmenu at intervals
    /// </summary>
    void Update()
    {

        if (inputDetected && warningActive)
        {

            inputDetected = false;
            warningActive = false;
            StartCoroutine(UnloadWarning());

        }

        if (Time.unscaledTime - timeSinceLastCheck >= intervalBetweenChecks)
        {

            timeSinceLastCheck = Time.unscaledTime;

            if (!IsMenuActive())
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

    }

    /// <summary>
    /// Checks for startmenu (and return-to-menu warning)
    /// </summary>
    /// <returns>True if startmenu is active</returns>
    private bool IsMenuActive()
    {

        int sceneAmount = SceneManager.sceneCount;

        for (int i = 0; i < sceneAmount; i++)
        {

            var scene = SceneManager.GetSceneAt(i);

            if (scene != null)
            {

                if (scene.name == startMenu)
                    return true;

                if (scene.name == returnToMenuWarning)
                    lastActivityTime = Time.unscaledTime;

            }

        }

        return false;

    }

    /// <summary>
    /// Loads warning screen
    /// </summary>
    /// <returns>Loading of scene</returns>
    private IEnumerator LoadWarning()
    {

        warningActive = true;

        yield return SceneManager.LoadSceneAsync(returnToMenuWarning, LoadSceneMode.Additive);

    }

    /// <summary>
    /// Unloads warning screen
    /// </summary>
    /// <returns>Unloading of scene</returns>
    private IEnumerator UnloadWarning()
    {

        yield return SceneManager.UnloadSceneAsync(returnToMenuWarning);

    }

    #endregion

}
