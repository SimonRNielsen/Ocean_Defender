using UnityEngine;

public class QuizManager : MonoBehaviour
{
    #region Fields
    public QuestionList quizData;
    public GameObject questionPrefab;
    public GameObject resultPrefab;
    public Transform questionParent;
    

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

        if (correct)
        {
            correctCount++;
        }

        //Clean the last panel
        foreach (Transform child in questionParent)
        {
            Destroy(child.gameObject);
        }

        //Go to next question
        questionIndex++;

        if (questionIndex >= quizData.questions.Length)
        {
            ShowResult();
            return;
        }

        ShowQuestion();
    }

    public void ShowResult()
    {
        GameObject panel = Instantiate(resultPrefab, questionParent);
        var ui = panel.GetComponent<ResultPanelUI>();
        ui.SetUp(correctCount, quizData.questions.Length, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
