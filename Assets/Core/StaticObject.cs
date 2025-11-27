using UnityEngine;
using System.Collections.Generic;


public class StaticObject : MonoBehaviour, IClickable
{
    #region Field
    //The Rigidbody for this Gameobject
    private Rigidbody2D rb;
    //To check if the GameObject is planted
    private bool isPlant = false;

    //The GameObject of the collision
    private GameObject go;

    [SerializeField, Tooltip("The sprite for when the eelgrass is planted")]
    public Sprite plantedEelgrass;
    //SpriteRenderer for the plantedEelgrass
    private SpriteRenderer rbSprite;
    private float spriteHeight;



    private bool inCollision = false;
    #endregion

    #region Properties

    #endregion

    #region Method
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Getting the component Rigidbody2D to make the GameObject move around
        rb = GetComponent<Rigidbody2D>();

        if (plantedEelgrass != null)
        {
            rbSprite = GetComponent<SpriteRenderer>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.LogWarning(inCollision);
    }
    public void OnPrimaryClick()
    {

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //Can move the object while it's hold if plant is false and isn't planted
        //if (inCollision == false)
        //{
        rb.position += (Vector2)movement;
        //}
    }

    public void OnPrimaryRelease()
    {
        //If the GameObject is going to be planted
        if (isPlant == true && rbSprite != null)
        {
            //Change the spirte
            rbSprite.sprite = plantedEelgrass;

            this.transform.localScale = Vector3.one * 0.15f;
            //Get the of the sprite
            spriteHeight = rbSprite.size.y /** 0.15f*/;

            Debug.LogWarning(spriteHeight);
            //Setting the position on top og the hole where it is planted
            this.gameObject.transform.position = (Vector2)go.transform.position + new Vector2(0, (spriteHeight / 2) * 0.15f);

            go.tag = "Untagged";

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inCollision = true;

        if (collision.CompareTag("Hole") == true && this.tag == "EelgrassNail")
        {
            go = collision.gameObject;
            isPlant = true;

            //Changing the collision tag so there can't be planted another Eelgrass in the same Hole
            collision.gameObject.tag = "Untagged";
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlant = false;
        inCollision = false;
    }

    #endregion
}
