using UnityEngine;

public class ClickTest : MonoBehaviour, IClickable
{

    public void OnPrimaryRelease()
    {

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.red;

    }

    public void OnPrimaryHold(Vector3 movement)
    {

        transform.position += movement;

    }

    public void OnPrimaryClick()
    {

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.green;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
