using UnityEngine;

public class BucketScript : MonoBehaviour, IClickable
{
    public GameObject eelgrassNail;
    private SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    public void OnPrimaryClick()
    {


        eelgrassNail.transform.position = (Vector2)this.transform.position + new Vector2(0, 2);

        Instantiate(eelgrassNail);

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //throw new System.NotImplementedException();
        movement = Vector3.zero;

        //rb.position += (Vector2)movement;
    }

    public void OnPrimaryRelease()
    {
        //throw new System.NotImplementedException();
    }

}
