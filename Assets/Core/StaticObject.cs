using UnityEngine;
using System.Collections.Generic;


public class StaticObject : MonoBehaviour, IClickable
{
    #region Field
    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    private float height;

    private bool canPlant = false;
    private GameObject go;

    public Sprite plantedEelgrass;
    private List<Sprite> sprites = new List<Sprite>();

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

            sprites.Add(plantedEelgrass);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPrimaryClick()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //Can move the object while it's hold if plant is false
        if (canPlant == false)
        {
            rb.position += (Vector2)movement;
        }
    }

    public void OnPrimaryRelease()
    {
        if (canPlant == true)
        {
            rbSprite.sprite = plantedEelgrass;

            height = rbSprite.size.y;

            this.transform.localScale = Vector3.one * 0.15f;

            this.gameObject.transform.position = (Vector2)go.transform.position + new Vector2(0, (height * 1.5f)/ 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole") == true && this.tag == "EelgrassNail")
        {
            go = collision.gameObject;
            canPlant = true;
            OnPrimaryRelease();

            collision.gameObject.tag = "Untagged";
        }
    }

    #endregion
}
