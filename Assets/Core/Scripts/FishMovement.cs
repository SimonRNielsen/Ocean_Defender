using System.Buffers.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishMovement : MonoBehaviour, IClickable
{
    [SerializeField] private float speed;
    [SerializeField] private float rightBorder = 10f;
    [SerializeField] private float leftBorder = -10f;
    [SerializeField] private float waitMin;
    [SerializeField] private float waitMax;

    [SerializeField] private float waveAmplitude = 0.5f;   // hvor højt den gynger
    [SerializeField] private float waveFrequency = 2f;     // hvor hurtigt den gynger
    private float baseY;                                   // udgangspunkt for y-position

    private bool movingRight = true;
    private float waitTimer = 0f;
    private float direction;
    private float delayStart = 0f;

    // beregn ny vandret position
    private float newX;

    // beregn lodret bølge
    private float newY;

    private SpriteRenderer sr;


    //Achievement
    //public bool clickedAchievement = false;
    [SerializeField, Tooltip("The achievement which needs to be in the scene")]
    public GameObject achievement;

    public void OnPrimaryClick()
    {
        //// Sæt achievement flag
        //clickedAchievement = true;

        //// Fjern fisken
        ////gameObject.SetActive(false);
        //Destroy(gameObject);
        //// eller Destroy(gameObject); hvis den skal fjernes permanent
        //Debug.Log("click");
        //sr.color = new Color(19f, 146f, 79f);
        //sr.color = Color.yellowGreen;

        //Setting the achievement as active
        achievement.SetActive(true);

        //Setting this object to not active
        this.gameObject.SetActive(false);

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //// Sæt achievement flag
        //clickedAchievement = true;

        //// Fjern fisken
        ////gameObject.SetActive(false);
        //Destroy(gameObject);
        //// eller Destroy(gameObject); hvis den skal fjernes permanent
        //Debug.Log("hold");

        

    }

    public void OnPrimaryRelease()
    {
        // Sæt achievement flag
        //clickedAchievement = true;

        // Fjern fisken
        //gameObject.SetActive(false);
        //Destroy(gameObject);
        // eller Destroy(gameObject); hvis den skal fjernes permanent

        //Debug.Log("release");
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseY = transform.position.y;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Test_Rikke")
        {
            delayStart = 5f;
        }

        //waitTimer = Random.Range(waitMin+delayStart, waitMax);
        RandomTime();
        baseY = transform.position.y;

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
        
        // beregn ny vandret position
        float newX = transform.position.x + direction * speed * Time.deltaTime;

        // beregn lodret bølge
        float newY = baseY + Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;

        // opdater position
        transform.position = new Vector3(newX, newY, transform.position.z);
        //transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (movingRight && transform.position.x > rightBorder)
        {
            movingRight = false;
            sr.flipX = !movingRight;
            //waitTimer = Random.Range(waitMin+delayStart, waitMax);
            RandomTime();
        }
        else if(!movingRight && transform.position.x < leftBorder)
        {
            movingRight = true;
            sr.flipX = !movingRight;
            //waitTimer = Random.Range(waitMin+delayStart, waitMax);
            RandomTime();
        }

    }

    public void RandomTime()
    {
        waitTimer = Random.Range(waitMin + delayStart, waitMax);
    }
}
