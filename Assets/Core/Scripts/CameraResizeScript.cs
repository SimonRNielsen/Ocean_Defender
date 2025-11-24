using UnityEngine;

public class CameraResizeScript : MonoBehaviour
{

    private float targetAspect = 16f / 9f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Camera cam = GetComponent<Camera>();
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        StartMenuScript.ButtonScale = scaleHeight;

        if (scaleHeight < 1f)
        {

            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;

        }
        else
        {

            float scaleWidth = 1f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
