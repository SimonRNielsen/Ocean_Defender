using UnityEngine;
using System.Collections.Generic;

public class PlacementScript : MonoBehaviour
{
    private List<Collider2D> collisions = new List<Collider2D>();
    private List<GameObject> objects = new List<GameObject>();

    [SerializeField, Tooltip("The time it takes to connect the eelgrass and nail")]
    public float connectTimer = 2f;
    private float timer = 0f;
    public GameObject eelgrassWithNail;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (collisions.Count == 2)
        {
            ConnectThem();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Nail" || collision.tag == "Eelgrass")
        {
            collisions.Add(collision);

            objects.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Nail" || collision.tag == "Eelgrass")
        {
            collisions.Remove(collision);

            objects.Remove(collision.gameObject);

            timer = 0f;
        }
    }

    private void ConnectThem()
    {
        timer += Time.deltaTime;

        if (timer > connectTimer)
        {
            while (objects.Count > 0)
            {
                objects[objects.Count - 1].gameObject.SetActive(false);
            }

            SpawnConnetedEelgrass();

            this.gameObject.SetActive(false);
        }

    }

    private void SpawnConnetedEelgrass()
    {
        eelgrassWithNail.SetActive(true);
        eelgrassWithNail.transform.position = Vector2.zero;

    }
}


