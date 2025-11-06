using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastScript : MonoBehaviour
{

    private Camera cam;
    private Vector3 oldMouseWorldPosition, newMouseWorldPosition;
    private IClickable dragThis;
    private bool primaryReleased, primaryPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        cam = GetComponent<Camera>();

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            throw new System.Exception("No camera found for RaycastScript, either no camera was detected as \"MainCamera\" or on object the script was attached to");

    }

    // Update is called once per frame
    void Update()
    {

        DeterminePrimaryInput();

        if (primaryReleased)
            OnReleaseAction();
        else if (primaryPressed)
            OnPressAction();

    }


    private void DeterminePrimaryInput()
    {

        primaryPressed = false;
        primaryReleased = false;
        Vector2 pointerPos;

        if (Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasReleasedThisFrame))
            pointerPos = Mouse.current.position.ReadValue();
        else if (Touchscreen.current != null && (Touchscreen.current.primaryTouch.press.isPressed || Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
            pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
        else
            return;

        Vector3 screenPosition = new Vector3(pointerPos.x, pointerPos.y, -cam.transform.position.z);
        newMouseWorldPosition = cam.ScreenToWorldPoint(screenPosition);

        if ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
            primaryReleased = true;
        else if ((Mouse.current != null && Mouse.current.leftButton.isPressed) || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed))
            primaryPressed = true;

    }


    private Vector3 Movement()
    {

        if (dragThis == null)
        {

            oldMouseWorldPosition = newMouseWorldPosition;
            return Vector3.zero;

        }

        Vector3 movement = newMouseWorldPosition - oldMouseWorldPosition;

        oldMouseWorldPosition = newMouseWorldPosition;

        return movement;

    }


    private void OnPressAction()
    {

        if (dragThis != null)
        {

            dragThis.OnPress(Movement());
            return;

        }

        RaycastHit2D hit = Physics2D.Raycast(newMouseWorldPosition, Vector2.zero);

        if (hit.collider != null)
        {

            dragThis = hit.transform.GetComponent<IClickable>();

            if (dragThis != null)
            {

                dragThis.OnPress(Movement());

            }

        }

    }


    private void OnReleaseAction()
    {

        if (dragThis != null)
        {

            dragThis.OnClick();
            dragThis = null;

        }
        else
        {

            RaycastHit2D hit = Physics2D.Raycast(newMouseWorldPosition, Vector2.zero);

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

    void OnPress(Vector3 movement);

}
