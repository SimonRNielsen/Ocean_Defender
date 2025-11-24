using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionPanelUI : MonoBehaviour
{
    //Fields
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;

    private Question currentQuestion;
    private QuizManager quizManager;

    public void Setup(Question q, QuizManager manager)
    {
        currentQuestion = q;
        quizManager = manager;

        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; //Capture index for button
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

    }

    void OnAnswerSelected(int index)
    {
        quizManager.SubmitAnswer(index == currentQuestion.correctAnswerIndex);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
