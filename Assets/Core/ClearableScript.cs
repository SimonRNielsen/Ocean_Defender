using UnityEngine;

public class ClearableScript : MonoBehaviour, IClickable
{
    #region Fields
    bool visible = true, collidingWithTrashCan = false;
    Renderer render;
    private Rigidbody2D rb;
    private ObjectPool pool;
    public ObjectPool Pool { get => pool; set => pool = value; }


    #endregion

    #region Methods 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPrimaryRelease()
    {
        rb.gravityScale = 0.02f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        //Reset opacity when released
        render.material.color = new Color(1f, 1f, 1f, 1f);

        //If object is trash and is released while colliding with trashcan, call the Recycle method.
        if (collidingWithTrashCan && CompareTag("Trash"))
        {
            Recycle();
            
        }

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //transform.position += movement;
        rb.position += (Vector2) movement;
    }

    public void OnPrimaryClick()
    {
        //rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        //Sets opacity so it is more seethrough when dragged
        render.material.color = new Color(1f, 1f, 1f, .5f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check wheter this object entered a TrashCan, and set the CollsidingWithTrashCan bool accordingly
        if(collision.CompareTag("TrashCan"))
        {
            collidingWithTrashCan = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Check wheter this object exited a TrashCan, and set the CollsidingWithTrashCan bool accordingly
        if (collision.CompareTag("TrashCan"))
        {
            collidingWithTrashCan = false;
        }
    }

    /// <summary>
    /// Method that handles what happens when trash is thrown out. 
    /// </summary>
    void Recycle()
    {
        //gameObject.SetActive(false);
        Release();
        //render.enabled = false; //blev brugt til tidlig test
        //TODO: Use object pool!
        //TODO: Handle score etc. 
    }



    public void Release()
    {
        pool.ReturnToPool(this);
    }
    #endregion
}
