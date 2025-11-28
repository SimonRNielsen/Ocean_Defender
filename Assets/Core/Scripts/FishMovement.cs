using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rightBorder = 10f;
    [SerializeField] private float leftBorder = -10f;
    [SerializeField] private float waitMin;
    [SerializeField] private float waitMax;

    private bool movingRight = true;
    private float waitTimer = 0f;
    private float direction;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        waitTimer = Random.Range(waitMin,waitMax);
    }

    

    // Update is called once per frame
    void Update()
    {
        if(waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        direction = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if(movingRight && transform.position.x > rightBorder)
        {
            movingRight = false;
            sr.flipX = !movingRight;
            waitTimer = Random.Range(waitMin,waitMax); 
        }
        else if(!movingRight && transform.position.x < leftBorder)
        {
            movingRight = true;
            sr.flipX = !movingRight;
            waitTimer = Random.Range(waitMin, waitMax);
        }

    }

   
}
