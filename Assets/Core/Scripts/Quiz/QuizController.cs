using UnityEngine;
using System.Collections;

public class QuizController : MonoBehaviour
{
    #region Fields
    public QuizManager quizManager; //Reference to QuizManager prefab
    //public float quizInterval = 30f; //Time between questions
    //public string sceneQuizName; //Name on the quiz to use

    private bool quizActive = false;
    #endregion
    #region Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    //IEnumerator QuizTimer()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(quizInterval);

    //        if (!quizActive)
    //        {
    //            ShowQuiz();
    //        }
    //    }
    //}

    //public void ShowQuiz()
    //{
    //    quizActive = true;
    //    Time.timeScale = 0f; //Pause the game

    //    quizManager.gameObject.SetActive(true);
    //    quizManager.StartQuiz(); //Call a new method 
    //}

    public void QuizEnded()
    {
        quizActive = false;
        quizManager.gameObject.SetActive(false);
        Time.timeScale = 1f; //Continue game
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
