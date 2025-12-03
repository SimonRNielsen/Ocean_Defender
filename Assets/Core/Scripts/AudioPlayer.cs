using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    private AudioSource audioSource;
    

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 1f;

        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioSource.volume = savedVolume;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnEnable()
    {

        //if (audioSource == null)
        //    audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {

            DataTransfer_SO.Instance.oneShotSoundEvent += PlayOneShotSound;
            DataTransfer_SO.Instance.loopingSoundEvent += PlayLoopingSound;
            DataTransfer_SO.Instance.playSoundEvent += Play;
            DataTransfer_SO.Instance.volumeChangedEvent += OnVolumeChanged;

            //audioSource.volume = 1f;
        }

    }


    private void OnDisable()
    {

        if (audioSource != null && DataTransfer_SO.Instance != null)
        {

            DataTransfer_SO.Instance.oneShotSoundEvent -= PlayOneShotSound;
            DataTransfer_SO.Instance.loopingSoundEvent -= PlayLoopingSound;
            DataTransfer_SO.Instance.playSoundEvent -= Play;
            DataTransfer_SO.Instance.volumeChangedEvent -= OnVolumeChanged;

        }

    }

    /// <summary>
    /// Plays soundclip once
    /// </summary>
    /// <param name="clip">Clip to be played</param>
    private void PlayOneShotSound(AudioClip clip)
    {

        if (clip == null)
        {

            Debug.LogWarning("No clip found");
            return;

        }

        audioSource.PlayOneShot(clip);

    }

    /// <summary>
    /// Loops sound
    /// </summary>
    /// <param name="clip">Clip to be played</param>
    private void PlayLoopingSound(AudioClip clip)
    {

        if (clip == null)
        {

            Debug.LogWarning("No clip found");
            return;

        }

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();

    }

    /// <summary>
    /// Plays sound
    /// </summary>
    /// <param name="clip">Clip to be played</param>
    private void Play(AudioClip clip)
    {

        if (clip == null)
        {

            Debug.LogWarning("No clip found");
            return;

        }

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();

    }

    private void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
    }

}
