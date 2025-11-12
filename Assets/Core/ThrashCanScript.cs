using UnityEngine;

public class ThrashCanScript : MonoBehaviour
{
    #region fields
    SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("The sprite of the trashcan when it is closed")] private Sprite closedSprite;
    [SerializeField, Tooltip("The sprite of the trashcan when it is open")] private Sprite openSprite;
    #endregion

    #region Methods

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When trash enters the TrashCan should change sprite to open
        if (collision.CompareTag("Trash"))
        {
            ChangeTrashCanSprite(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //When trash exits the trahscan should change sprite to closed.
        if (collision.CompareTag("Trash"))
        {
            ChangeTrashCanSprite(true);
        }
    }

    /// <summary>
    /// Changes the sprite of the TrashCan, to either its closedSprite or openSprite
    /// </summary>
    /// <param name="Open"> Whether the trashcan is currently open </param>
    private void ChangeTrashCanSprite(bool Open)
    {
        if (closedSprite != null && openSprite != null)
        {
            if (Open)
            {
                spriteRenderer.sprite = closedSprite;
            }
            else
            {
                spriteRenderer.sprite = openSprite;
            }
        }
        else
        {
            Debug.Log("You must set both an open and close sprite for the TrashCan, for it to change sprite!");
        }
    }
    #endregion
}
