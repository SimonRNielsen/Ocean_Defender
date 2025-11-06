using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastScript : MonoBehaviour
{

    #region Fields

    [Header("Input settings"), Space]
    [Tooltip("Make object only interactable if input was initiated on it"), SerializeField] private bool primaryInitialPressIsSelector = false;
    [Tooltip("Disable action when input is just pressed"), SerializeField] private bool disableOnPress = false;
    [Tooltip("Disable action when input is held"), SerializeField] private bool disableOnHold = false;
    [Tooltip("Disable action when input is released"), SerializeField] private bool disableOnRelease = false;

    private Camera cam;
    private Vector3 oldPointerWorldPosition, newPointerWorldPosition;
    private Vector2 pointerPos;
    private IClickable selectedObject;
    private bool primaryIsReleased, primaryIsHeld, primaryIsPressed, actionDetectedThisFrame, touchEnabled, mouseEnabled;

    #endregion
    #region Properties

    /// <summary>
    /// Calculates difference between previous frames recorded position and current frame and stores this as a movement value in the Vector3 "movement"
    /// </summary>
    private Vector3 Movement
    {
        get
        {

            if (selectedObject == null) //Ensures no movement if object-dragging was initiated this frame
            {

                oldPointerWorldPosition = newPointerWorldPosition;
                return Vector3.zero;

            }

            Vector3 move = newPointerWorldPosition - oldPointerWorldPosition;

            oldPointerWorldPosition = newPointerWorldPosition;

            return move;

        }
    }

    #endregion
    #region Constructor



    #endregion
    #region Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        cam = GetComponent<Camera>();

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            throw new System.Exception("No camera found for RaycastScript, either no camera was detected as \"MainCamera\" or on object the script was attached to");

        CheckForPeripherals();

    }

    // Update is called once per frame
    void Update()
    {

        if (DeterminePrimaryInput())
            UpdateMouseWorldPosition();

        if (primaryIsPressed)
        {

            OnPrimaryPress();
            oldPointerWorldPosition = newPointerWorldPosition;

        }
        else if (primaryIsReleased)
            OnReleasePrimaryAction();
        else if (primaryIsHeld)
            OnHoldingPrimaryAction();

    }

    /// <summary>
    /// Updates pointers world position
    /// </summary>
    private void UpdateMouseWorldPosition()
    {

        Vector3 screenPosition = new Vector3(pointerPos.x, pointerPos.y, -cam.transform.position.z);
        newPointerWorldPosition = cam.ScreenToWorldPoint(screenPosition);

    }

    /// <summary>
    /// Detects if either touch or mouse press was input
    /// </summary>
    /// <returns>True if any detected</returns>
    private bool DeterminePrimaryInput()
    {

        primaryIsHeld = false;
        primaryIsReleased = false;
        primaryIsPressed = false;
        actionDetectedThisFrame = false;

        return CheckLeftMouse() || CheckPrimaryTouch();

    }

    /// <summary>
    /// Checks for input from mouse leftbutton and records position thereof if so
    /// </summary>
    /// <returns>True if actuated</returns>
    private bool CheckLeftMouse()
    {

        if (!mouseEnabled && Mouse.current == null) return false;

        primaryIsHeld = Mouse.current.leftButton.isPressed;
        primaryIsReleased = Mouse.current.leftButton.wasReleasedThisFrame;
        primaryIsPressed = Mouse.current.leftButton.wasPressedThisFrame;

        bool actionDetected = PrimaryActionDetected();
        if (actionDetected && !actionDetectedThisFrame)
        {

            pointerPos = Mouse.current.position.ReadValue();
            actionDetectedThisFrame = true;

        }

        return actionDetected;

    }

    /// <summary>
    /// Checks for input from touchscreen with single-finger input and records position thereof if so
    /// </summary>
    /// <returns>True if actuated</returns>
    private bool CheckPrimaryTouch()
    {

        if (!touchEnabled && Touchscreen.current == null) return false;

        primaryIsHeld = Touchscreen.current.primaryTouch.press.isPressed;
        primaryIsReleased = Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        primaryIsPressed = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        bool actionDetected = PrimaryActionDetected();
        if (actionDetected && !actionDetectedThisFrame)
        {

            pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
            actionDetectedThisFrame = true;

        }

        return actionDetected;

    }

    /// <summary>
    /// Logic for a drag-effect (or hold effect if initial press isn't set as only way of selecting)
    /// </summary>
    private void OnHoldingPrimaryAction()
    {

        if (disableOnHold) return;

        if (selectedObject != null)
        {

            selectedObject.OnPrimaryHold(Movement);
            return;

        }

        if (primaryInitialPressIsSelector) return;

        IClickable target = CastRay();

        if (target != null)
        {

            target.OnPrimaryHold(Movement);
            selectedObject = target;

        }

    }

    /// <summary>
    /// Logic for when primary input is released
    /// </summary>
    private void OnReleasePrimaryAction()
    {

        if (disableOnRelease) return;

        if (selectedObject != null)
        {

            selectedObject.OnPrimaryRelease();
            selectedObject = null;

        }
        else if (!primaryInitialPressIsSelector)
        {

            IClickable target = CastRay();

            if (target != null)
                target.OnPrimaryRelease();

        }

    }

    /// <summary>
    /// Logic for when primary input is initially actuated
    /// </summary>
    private void OnPrimaryPress()
    {

        if (disableOnPress) return;

        IClickable target = CastRay();

        if (target != null)
        {

            target.OnPrimaryClick();
            selectedObject = target;

        }

    }

    /// <summary>
    /// Tries to determine which kind of input is present
    /// </summary>
    private void CheckForPeripherals()
    {

        touchEnabled = Touchscreen.current != null;
        mouseEnabled = Mouse.current != null;

    }

    /// <summary>
    /// Scans for an objects collisionbox on the point where the pointer is located in world-space
    /// </summary>
    /// <returns>Objects IClickable-inheriting class - if any, otherwise null</returns>
    private IClickable CastRay()
    {

        RaycastHit2D hit = Physics2D.Raycast(newPointerWorldPosition, Vector2.zero);

        if (hit.collider != null)
            return hit.transform.GetComponent<IClickable>();
        else
            return null;

    }

    /// <summary>
    /// Checks if any of the primary input was detected this frame
    /// </summary>
    /// <returns>True, if any</returns>
    private bool PrimaryActionDetected() => primaryIsHeld || primaryIsPressed || primaryIsReleased;

    #endregion

}

/// <summary>
/// Interface to interpert input
/// </summary>
public interface IClickable
{

    /// <summary>
    /// Triggered on object when primary input is released
    /// </summary>
    void OnPrimaryRelease();

    /// <summary>
    /// Triggered on object when primary input is held, takes a Vector3 
    /// </summary>
    /// <param name="movement">Vector3 direct difference in value from mouses last recorded frame "transform.position += movement;" moves object along with mouse</param>
    void OnPrimaryHold(Vector3 movement);

    /// <summary>
    /// Triggered on object when primary input is initially actuated
    /// </summary>
    void OnPrimaryClick();

}
