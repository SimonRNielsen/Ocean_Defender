using UnityEngine;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    #region Fields
    public QuestionList quizData;
    public GameObject questionPrefab;
    public GameObject resultPrefab;
    public Transform questionParent;
    public QuizController quizController;
    

    int questionIndex = 0;
    int correctCount = 0;
    #endregion

    #region Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (questionIndex >= quizData.questions.Length)
        {
            Debug.Log("Quiz Done!");
            ShowResult();
            return;
        }

        //Make a panel
        GameObject panel = Instantiate(questionPrefab, questionParent);

        //Get the UI script
        var ui = panel.GetComponent<QuestionPanelUI>();

        //Give the question and reference to the manager
        ui.Setup(quizData.questions[questionIndex], this);
    }

    public void SubmitAnswer(bool correct)
    {
        Debug.Log(correct ? "Correct!" : "Wrong!");

        StartCoroutine(HandleAnswer(correct));
        //ShowQuestion();
    }

    public void ShowResult()
    {
        GameObject panel = Instantiate(resultPrefab, questionParent);
        var ui = panel.GetComponent<ResultPanelUI>();
        ui.SetUp(correctCount, quizData.questions.Length, this);

        StartCoroutine(CloseQuizAfterDelay());
    }

    private IEnumerator CloseQuizAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f); //Realtid - Timescale doesnt work on this
        quizController.QuizEnded();
    }
    private IEnumerator HandleAnswer(bool correct)
    {
        //Wait for 2 sekunds
        yield return new WaitForSeconds(2f);

        if (correct)
        {
            correctCount++;
        }

        //Remove last panel
        foreach (Transform child in questionParent)
        {
            Destroy(child.gameObject);
        }

        //Go to next question 
        questionIndex++;

        if (questionIndex >= quizData.questions.Length)
        {
            ShowResult();
        }
        else
        {
            ShowQuestion();
        }

    }

    public void StartQuiz()
    {
        questionIndex = 0;
        correctCount = 0;

        foreach (Transform child in questionParent)
        {
            Destroy(child.gameObject);
        }

        ShowQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
