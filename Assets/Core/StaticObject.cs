using UnityEngine;

public class StaticObject : MonoBehaviour, IClickable
{
    #region Field
    private Rigidbody2D rb;
    //private SpriteRenderer rbSprite;
    #endregion

    #region Properties

    #endregion

    #region Method
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //rbSprite = GetComponent<SpriteRenderer>();
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
        //Can move the object while it's hold
        rb.position += (Vector2)movement;
    }

    public void OnPrimaryRelease()
    {
        //throw new System.NotImplementedException();
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("Collision");

    //    Debug.LogAssertion("Collision");

    //    GameObject go = collision.gameObject;

    //    if (go != null)
    //        Debug.LogWarning("No object found");

    //    SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

    //    if (spriteRenderer != null)
    //    {
    //        spriteRenderer.color = Color.green;
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{

    //    GameObject go = collision.gameObject;

    //    SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

    //    if (spriteRenderer != null)
    //    {
    //        spriteRenderer.color = Color.yellow;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision on");

        Debug.LogAssertion("Collision on");


        GameObject go = collision.gameObject;

        if (go != null)
            Debug.LogWarning("No object found");
        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Collision off");

        Debug.LogAssertion("Collision off");


        GameObject go = collision.gameObject;

        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }
    }
    #endregion
}
