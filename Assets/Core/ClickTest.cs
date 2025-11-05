using UnityEngine;

public class ClickTest : MonoBehaviour, IClickable
{
    public void OnClick()
    {

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.red;

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
