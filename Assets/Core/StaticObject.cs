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

    [SerializeField, Tooltip("The sprite for when the eelgrass is planted")]
    public Sprite plantedEelgrass;
    //SpriteRenderer for the plantedEelgrass
    private SpriteRenderer rbSprite;
    private float spriteHeight;

    private ScoreCounterScript scoreCounter; //The Scorecounter used to add a score when the objects is cleared.
    [SerializeField, Tooltip("The score awarded when clearing the clearable object")] private int score = 0;


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


        scoreCounter = FindAnyObjectByType<ScoreCounterScript>();
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
            this.gameObject.transform.position = (Vector2)go.transform.position + new Vector2(0.2f, (spriteHeight / 2) * 0.15f + .1f);

            go.tag = "Untagged";

            score++;
            isPlanted();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole") == true && this.tag == "EelgrassNail")
        {
            go = collision.gameObject;
            canPlant = true;

            //Changing the collision tag so there can't be planted another Eelgrass in the same Hole
            collision.gameObject.tag = "Untagged";
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        canPlant = false;
    }

    private void isPlanted()
    {
        //Sending the score to the UI
        scoreCounter.AddToScore(score);
    }


    #endregion
}
