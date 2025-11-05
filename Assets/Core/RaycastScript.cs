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

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {

            pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();

        }
        else
        {

            pointerPos = Mouse.current.position.ReadValue();

        }


        Vector3 sceenPosition = new Vector3(pointerPos.x, pointerPos.y, 100f);
        Vector3 worldPosition = cam.ScreenToWorldPoint(sceenPosition);

        bool pressed;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {

            pressed = true;

        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.tapCount.ReadValue() > 0)
        {

            pressed = true;

        }
        else
        {

            pressed = false;

        }

        if (pressed)
        {

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {

                IClickable clickable = hit.transform.GetComponent<IClickable>();
                if (clickable != null)
                    clickable.OnClick();

            }

        }

    }
}


public interface IClickable
{

    void OnClick();

}
