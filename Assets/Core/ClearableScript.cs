using UnityEngine;

public class ClearableScript : MonoBehaviour, IClickable
{
    #region Field
    bool visibel = true;
    Renderer render;

    #endregion


    public void OnPrimaryRelease()
    {
        //When click it will be unvisibel
        if (render.isVisible == true)
        {
            render.enabled = false;
        }

        //When clicked will it be visibel/unvisibel 
        //if (visibel == true)
        //{
        //    render.enabled = false;
        //    visibel = false;
        //}
        //else if (visibel == false)
        //{
        //    render.enabled = true;
        //    visibel = true;
        //}
    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrimaryClick()
    {
        //throw new System.NotImplementedException();
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
