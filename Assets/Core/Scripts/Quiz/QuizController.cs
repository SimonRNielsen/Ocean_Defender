using UnityEngine;
using System.Collections;

public class QuizController : MonoBehaviour
{
    #region Fields
    public QuizManager quizManager; //Reference to QuizManager prefab

    private bool quizActive = false;
    #endregion
    #region Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void QuizEnded()
    {
        quizActive = false;
        quizManager.gameObject.SetActive(false);
        //Time.timeScale = 1f; //Continue game
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
