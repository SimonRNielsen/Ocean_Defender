using UnityEngine;

public class ClearableScript : MonoBehaviour, IClickable
{
    #region Field
    bool visibel = true;
    Renderer render;

    #endregion


    public void OnClick()
    {
        if (visibel == true)
        {
            render.enabled = false;
            visibel = false;
        }
        else if (visibel == false)
        {
            render.enabled = true;
            visibel = true;
        }
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
