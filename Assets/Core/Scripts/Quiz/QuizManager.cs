using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class QuizManager : MonoBehaviour
{
    #region Fields
    public QuestionList quizData;
    public GameObject questionPrefab;
    public GameObject resultPrefab;
    public Transform questionParent;
    public QuizController quizController;

    public float questionInterval = 1f;

    public GameObject factBoxPanel;
    public TextMeshProUGUI factText;

    private int questionIndex = 0;
    private int correctCount = 0;

    private bool showingResult = false;

    [SerializeField, Tooltip("Audioclip for correct answear")] private AudioClip correctAudio;
    [SerializeField, Tooltip("Audioclip for wrong answear")] private AudioClip wrongAudio;

    [SerializeField, Tooltip("Achievement KlogeMåge")] private GameObject achievement;
    #endregion

    #region Methods

    private void ShowQuestion()
    {
        Time.timeScale = 0f; //Game pauses when question from quiz is shown
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

    }

    public void ShowResult()
    {

        ShowResultInFactBox(correctCount, quizData.questions.Length);
    }

    public void ShowResultInFactBox(int score, int total)
    {
        showingResult = true;
        factText.text = $"Du fik {score} ud af {total} rigtige!";
        factBoxPanel.SetActive(true);

        SetupFactBoxContinueButton(OnResultContinueClicked);

        //if (score > 3)
        //{
        //    achievement.SetActive(true);
        //}

        DataTransfer_SO.Instance.QuizScore = score;

        Time.timeScale = 0f;
    }

    private void OnResultContinueClicked()
    {
        factBoxPanel.SetActive(false);
        Time.timeScale = 1f;

        showingResult = false;

        //Undload quizScene
        quizController.QuizEnded();
    }
    private IEnumerator CloseQuizAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f); //Realtime - Timescale doesnt work on this

        Time.timeScale = 1f; //Game continues after the quiz is done

        quizController.QuizEnded();
    }
    private IEnumerator HandleAnswer(bool correct)
    {
        //Wait for 2 sekunds
        yield return new WaitForSecondsRealtime(2f);

        if (correct)
        {
            correctCount++;
        }

        //Remove last panel
        foreach (Transform child in questionParent)
        {
            Destroy(child.gameObject);
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

    public IEnumerator ProceedAfterFactBox()
    {
        //Start game again
        Time.timeScale = 1f;

        //Start timer once factabox closes
        yield return new WaitForSecondsRealtime(questionInterval);

        //Pause till next question
        Time.timeScale = 0f;

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

    public void ShowFactBox(string text)
    {
        factBoxPanel.SetActive(true);

        if (factText != null)
        {
            factText.text = text;
        }
    }

    public void SetupFactBoxContinueButton(UnityEngine.Events.UnityAction action)
    {
        Button continueBtn = factBoxPanel.GetComponentInChildren<Button>();
        if (continueBtn != null)
        {
            continueBtn.onClick.RemoveAllListeners();
            continueBtn.onClick.AddListener(action);
        }
    }
    #endregion
}
