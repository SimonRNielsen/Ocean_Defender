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
                instance = CreateInstance<DataTransfer_SO>();

            return instance;

        }

    }

    #endregion
    #region Events

    public Action<AudioClip> oneShotSoundEvent;
    public Action<AudioClip> loopingSoundEvent;
    public Action<AudioClip> playSoundEvent;

    #endregion

}
