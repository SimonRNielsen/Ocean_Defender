using UnityEngine;

public class TrashFloatOrganic : MonoBehaviour
{
    [Header("Bevægelsesstyrke")]
    public float horizontalStrength = 1.0f;   // Hvor meget sidebevægelse
    public float verticalWobbleStrength = 0.2f; // Hvor meget det "hopper"/vugger lidt op og ned

    [Header("Hastighed")]
    public float horizontalSpeed = 0.5f;  // Hvor hurtigt det driver til siden
    public float verticalSpeed = 0.8f;    // Hvor hurtigt wobble ændrer sig

    [Header("Rotation")]
    public float rotationStrength = 30f;  // Hvor meget rotation
    public float rotationSpeed = 0.8f;    // Hvor hurtigt rotationen skifter

    private float seedX;
    private float seedY;
    private float seedR;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Random seed for hvert objekt så de ikke følger samme mønster
        seedX = Random.Range(0f, 999f);
        seedY = Random.Range(0f, 999f);
        seedR = Random.Range(0f, 999f);
    }

    void FixedUpdate()
    {
        // --- ORGANISK SIDE-BEVÆGELSE ---
        float noiseX = Mathf.PerlinNoise(seedX, Time.time * horizontalSpeed);
        float xOffset = (noiseX - 0.5f) * 2f * horizontalStrength; //Tager input med

        // --- WOBBLE OP OG NED ---
        float noiseY = Mathf.PerlinNoise(seedY, Time.time * verticalSpeed);
        float yOffset = (noiseY - 0.5f) * 2f * verticalWobbleStrength;//Tager input med

        // Tilføj driften til objektets position
        rb.position += new Vector2(xOffset, yOffset) * Time.fixedDeltaTime;//Tager input med

        // --- ORGANISK ROTATION ---
        float noiseRot = Mathf.PerlinNoise(seedR, Time.time * rotationSpeed);//Tager input med
        float rot = (noiseRot - 0.5f) * 2f * rotationStrength;//Tager input med
        rb.rotation += rot * Time.fixedDeltaTime;
    }
}
