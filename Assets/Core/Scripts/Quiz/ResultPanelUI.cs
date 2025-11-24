using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultPanelUI : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI scoreText;
    #endregion

    #region Methods
    public void SetUp(int score, int total, QuizManager manager)
    {
        headerText.text = "Quix Fuldendt";
        scoreText.text = $"Du fik {score} ud af {total} rigtige!";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
