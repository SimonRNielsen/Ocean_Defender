using UnityEngine;

public class ClickTest : MonoBehaviour, IClickable
{

    private Renderer rend;

    public void OnPrimaryRelease()
    {

        if (rend != null)
            rend.material.color = Color.red;

    }

    public void OnPrimaryHold(Vector3 movement)
    {

        transform.position += movement;

    }

    public void OnPrimaryClick()
    {

        if (rend != null)
            rend.material.color = Color.green;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rend = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
