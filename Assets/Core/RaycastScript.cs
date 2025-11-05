using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastScript : MonoBehaviour
{

    Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        cam = GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {

        Vector2 pointerPos;

        if (Touchscreen.current != null && (Touchscreen.current.primaryTouch.press.isPressed || Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
            pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasReleasedThisFrame))
            pointerPos = Mouse.current.position.ReadValue();
        else
            return;

        Vector3 sceenPosition = new Vector3(pointerPos.x, pointerPos.y, 100f);
        Vector3 worldPosition = cam.ScreenToWorldPoint(sceenPosition);

        bool pressed = false, released = false;

        if ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
            released = true;
        else if ((Mouse.current != null && Mouse.current.leftButton.isPressed) || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed))
            pressed = true;

        if (released)
        {

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {

                IClickable clickable = hit.transform.GetComponent<IClickable>();

                if (clickable != null)
                    clickable.OnClick();

            }

        }
        else if (pressed)
        {

            //RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            //if (hit.collider != null)
            //{

            //    IClickable clickable = hit.transform.GetComponent<IClickable>();

            //    if (clickable != null)
            //        clickable.OnPress();

            //}

        }

    }

}


public interface IClickable
{

    void OnClick();

    //void OnPress();

}
