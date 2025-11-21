using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementScript : MonoBehaviour, IClickable
{
    
    #region Field


    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPrimaryClick()
    {
        //Moving the achievement to the rigth side of the screen to be seen through the rest of the game
        transform.position = new Vector3(8, 2, 0);
        transform.localScale = Vector3.one / 4;
    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrimaryRelease()
    {
        //throw new System.NotImplementedException();
    }
}
