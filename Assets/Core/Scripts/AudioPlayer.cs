using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    private AudioSource audioSource;


    private void Awake()
    {

        DontDestroyOnLoad(gameObject);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {

            DataTransfer_SO.Instance.oneShotSoundEvent += PlayOneShotSound;
            DataTransfer_SO.Instance.loopingSoundEvent += PlayLoopingSound;
            DataTransfer_SO.Instance.playSoundEvent += Play;

        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void PlayOneShotSound(AudioClip clip)
    {

        if (clip == null)
        {

            Debug.LogWarning("No clip found");
            return;
            
        }
        
        audioSource.PlayOneShot(clip);

    }


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

}
