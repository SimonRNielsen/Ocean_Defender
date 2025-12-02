using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementScript : MonoBehaviour, IClickable
{

    #region Field
    private float timeLeft;

    [SerializeField, Tooltip("Is the achievement a FISKEVISKER")]
    public bool fiskeVisker = false;
    [SerializeField, Tooltip("Is the achievement a KLOGEMAAGE")]
    public bool klogemaage = false;

    private float moveTimer = 0;
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = DataTransfer_SO.Instance.roundTimeRemaining;

        if (timeLeft <= 0)
        {
            transform.position = new Vector3(5, 0, 0);
            transform.localScale = Vector3.one;

            if (fiskeVisker == true)
            {
                transform.position = new Vector3(-5, 0, 0);
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
    }
}
