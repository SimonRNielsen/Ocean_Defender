using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public enum Achievements
{

    Fiskeviskeren,
    KlogeMaage,
    Oprydning,
    Bundtrawl,
    Aalegraes

}

public class AchievementScript : MonoBehaviour, IClickable
{

    #region Field
    private float timeLeft;

    [SerializeField, Tooltip("Is the achievement a FISKEVISKER")]
    public bool fiskeVisker = false;
    [SerializeField, Tooltip("Is the achievement a KLOGEMAAGE")]
    public bool klogemaage = false;

    private float moveTimer = 0;
    [SerializeField] private Achievements id;
    [SerializeField, Tooltip("Achievement sound")] private AudioClip achievementSound;
    #endregion

    #region Properties

    public int ID { get => (int)id; }

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (klogemaage == true)
        {
            transform.position = new Vector3(5, 0, 0);
        }
        else
        {
            transform.position = Vector3.zero;
        }

        DataTransfer_SO.Instance.oneShotSoundEvent(achievementSound);

    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = DataTransfer_SO.Instance.roundTimeRemaining;

        if (timeLeft <= 1)
        {
            transform.position = new Vector3(5, 0, 0);
            transform.localScale = Vector3.one;

            if (fiskeVisker == true)
            {
                transform.position = new Vector3(-5, 2.6f, 0);
            }

            if (klogemaage == true)
            {
                transform.position = new Vector3(-5, -2.5f, 0);
            }
        }

        //If the player doesn't interact with the achievement it will move to the side
        moveTimer += Time.deltaTime;

        if (moveTimer > 1.5f)
        {
            newPositionSmall();
        }
    }

    public void OnPrimaryClick()
    {
        newPositionSmall();
    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrimaryRelease()
    {
        //throw new System.NotImplementedException();
    }

    private void newPositionSmall()
    {
        //Moving the achievement to the rigth side of the screen to be seen through the rest of the game
        transform.position = new Vector3(8, 2, 0);
        transform.localScale = Vector3.one / 4;

        if (fiskeVisker == true)
        {
            transform.position = new Vector3(8, 0, 0);
        }

        if (klogemaage == true)
        {
            transform.position = new Vector3(8, -2, 0);
        }
    }
}
