using UnityEngine;

public class ClearableScript : MonoBehaviour, IClickable
{
    #region Fields
    bool collidingWithTrashCan = false;
    Renderer render;
    private Rigidbody2D rb;
    private ObjectPool pool;
    [SerializeField, Tooltip("The score awarded when clearing the clearable object")] private int score =0;
    private ScoreCounterScript scoreCounter; //The Scorecounter used to add a score when the objects is cleared.
    #endregion

    #region Properties
    public ObjectPool Pool { get => pool; set => pool = value; }
    public int Score { get  => score; private set => score = value; }
    #endregion

    #region Methods 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();
        scoreCounter = FindAnyObjectByType<ScoreCounterScript>();
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
        if (collision.CompareTag("TrashCan"))
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
        if(scoreCounter != null)
        {
            scoreCounter.AddToScore(score);
        }
        else
        {
            Debug.Log("No ScoreCounter found...");
        }
            Release();
        //render.enabled = false; //blev brugt til tidlig test
        //TODO: Use object pool!
    }



    public void Release()
    {
        pool.ReturnToPool(this);
    }
    #endregion
}
