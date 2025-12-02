using UnityEngine;
using UnityEngine.UIElements;

public class AnimElement : MonoBehaviour
{
    private VisualElement ve;
    [SerializeField] private Sprite animImg;
    private Vector2 pos;


    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        ve = root.Q<VisualElement>("Arrow");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // ve.style.backgroundImage = new StyleBackground(animImg);

    }
}
