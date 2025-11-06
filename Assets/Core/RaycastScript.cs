using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastScript : MonoBehaviour
{

    private Camera cam;
    private Vector3 oldPointerWorldPosition, newPointerWorldPosition, movement;
    private Vector2 pointerPos;
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

        if (DeterminePrimaryInput())
            UpdateMouseWorldPosition();

        if (primaryReleased)
            OnReleasePrimaryAction();
        else if (primaryPressed)
            OnPressingPrimaryAction();

    }


    private void UpdateMouseWorldPosition()
    {

        Vector3 screenPosition = new Vector3(pointerPos.x, pointerPos.y, -cam.transform.position.z);
        newPointerWorldPosition = cam.ScreenToWorldPoint(screenPosition);

    }


    private bool DeterminePrimaryInput()
    {

        primaryPressed = false;
        primaryReleased = false;

        return CheckLeftMouse() || CheckPrimaryTouch();

    }


    private bool CheckLeftMouse()
    {

        if (Mouse.current == null) return false;

        primaryPressed = Mouse.current.leftButton.isPressed;
        primaryReleased = Mouse.current.leftButton.wasReleasedThisFrame;

        bool temp = primaryReleased || primaryPressed;
        if (temp)
            pointerPos = Mouse.current.position.ReadValue();

        return temp;

    }


    private bool CheckPrimaryTouch()
    {

        if (Touchscreen.current == null) return false;

        primaryPressed = Touchscreen.current.primaryTouch.press.isPressed;
        primaryReleased = Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;

        bool temp = primaryReleased || primaryPressed;
        if (temp)
            pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();

        return temp;

    }


    private Vector3 Movement()
    {

        if (dragThis == null)
        {

            oldPointerWorldPosition = newPointerWorldPosition;
            return Vector3.zero;

        }

        movement = newPointerWorldPosition - oldPointerWorldPosition;

        oldPointerWorldPosition = newPointerWorldPosition;

        return movement;

    }


    private void OnPressingPrimaryAction()
    {

        if (dragThis != null)
        {

            dragThis.OnPress(Movement());
            return;

        }

        RaycastHit2D hit = Physics2D.Raycast(newPointerWorldPosition, Vector2.zero);

        if (hit.collider != null)
        {

            IClickable clickable = hit.transform.GetComponent<IClickable>();

            if (clickable != null)
            {

                clickable.OnPress(Movement());
                dragThis = clickable;

            }

        }

    }


    private void OnReleasePrimaryAction()
    {

        if (dragThis != null)
        {

            dragThis.OnClick();
            dragThis = null;

        }
        else
        {

            RaycastHit2D hit = Physics2D.Raycast(newPointerWorldPosition, Vector2.zero);

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
