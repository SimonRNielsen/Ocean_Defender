using UnityEngine;
using System.Collections.Generic;


public class StaticObject : MonoBehaviour, IClickable
{
    #region Field
    //The Rigidbody for this Gameobject
    private Rigidbody2D rb;
    //To check if the GameObject is planted
    private bool canPlant = false;

    //The GameObject of the collision
    private GameObject go;
    //Temp GameObject when there isn't no more collision
    //private GameObject tmpGo = new GameObject();

    [SerializeField, Tooltip("The sprite for when the eelgrass is planted")]
    public Sprite plantedEelgrass;
    //SpriteRenderer for the plantedEelgrass
    private SpriteRenderer rbSprite;
    private float spriteHeight;

    //private ScoreCounterScript scoreCounter; //The Scorecounter used to add a score when the objects is cleared.
    [SerializeField, Tooltip("The score awarded when clearing the clearable object")] private int score = 0;

    [SerializeField] private ScoreCounterScript scoreCounter;

    #endregion

    #region Properties

    #endregion

    #region Method
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Getting the component Rigidbody2D to make the GameObject move around
        rb = GetComponent<Rigidbody2D>();

        rbSprite = GetComponent<SpriteRenderer>();


        //scoreCounter = FindAnyObjectByType<ScoreCounterScript>();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPrimaryClick()
    {

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //Can move the object while it's hold if plant is false and isn't planted
        if (rbSprite.sprite != plantedEelgrass)
        {
            rb.position += (Vector2)movement;
            //rb.MovePosition(rb.position + (Vector2)movement);
        }
    }

    public void OnPrimaryRelease()
    {
        //If the GameObject is going to be planted
        if (canPlant == true && rbSprite.sprite != plantedEelgrass)
        {
            //Change the spirte
            rbSprite.sprite = plantedEelgrass;

            this.transform.localScale = Vector3.one * 0.15f;
            //Get the of the sprite
            spriteHeight = rbSprite.size.y * 0.15f;

            //Setting the position on top og the hole where it is planted
            this.gameObject.transform.position = (Vector2)go.transform.position + new Vector2(0.2f, (spriteHeight / 2) * 0.15f + .1f) + new Vector2(-1.42091f + 1.18f, 0.9648225f - 0.84f);

            //Changing the collision tag so there can't be planted another Eelgrass in the same Hole
            go.tag = "Untagged";

            //score++;
            isPlanted();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole") == true && this.tag == "EelgrassNail")
        {
            go = collision.gameObject;
            canPlant = true;

        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.CompareTag("Hole"))
        //{
        //    canPlant = false;
        //}
        //go = tmpGo;
    }

    private void isPlanted()
    {
        //Sending the score to the UI
        //scoreCounter.AddToScore(score);
        scoreCounter.AddToScore(1);
    }


    #endregion
}
