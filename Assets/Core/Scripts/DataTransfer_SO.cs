using System;
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

    public Action<AudioClip> oneShotSoundEvent;
    public Action<AudioClip> loopingSoundEvent;
    public Action<AudioClip> playSoundEvent;
    public Action resetEvent;

    #endregion

    /// <summary>
    /// Resets events to avoid memory leaks
    /// </summary>
    private void OnEnable()
    {

        oneShotSoundEvent = null;
        loopingSoundEvent = null;
        playSoundEvent = null;
        resetEvent = null;

    }

}
