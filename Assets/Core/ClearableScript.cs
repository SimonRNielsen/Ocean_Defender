using UnityEngine;

public class ClearableScript : MonoBehaviour, IClickable
{
    #region Fields
    bool visible = true, collidingWithTrashCan = false;
    Renderer render;

    private ObjectPool pool;
    public ObjectPool Pool { get => pool; set => pool = value; }

    #endregion

    #region Methods 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPrimaryRelease()
    {
        //Reset opacity when released
        render.material.color = new Color(1f, 1f, 1f, 1f);

        //If object is trash and is released while colliding with trashcan, call the Recycle method.
        if (collidingWithTrashCan && CompareTag("Trash"))
        {
            render.enabled = false;
            Release();
            Recycle();
        }

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        transform.position += movement;
    }

    public void OnPrimaryClick()
    {
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
        gameObject.SetActive(false);

        //TODO: Use object pool!
        //TODO: Handle score etc. 
    }



    public void Release()
    {
        pool.ReturnToPool(this);
    }
    #endregion
}
