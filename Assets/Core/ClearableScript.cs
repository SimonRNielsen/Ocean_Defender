using UnityEngine;

public class ClearableScript : MonoBehaviour, IClickable
{
    #region Field
    bool visible = true;
    Renderer render;

    #endregion


    public void OnPrimaryRelease()
    {
        //When click it will be unvisibel
        //if (render.isVisible == true)
        //{
        //    render.enabled = false;
        //}
        render.material.color = Color.white;

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        transform.position += movement;
    }

    public void OnPrimaryClick()
    {
        //set opacity so it is more seethrough
        render.material.color = Color.yellow;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
