using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuestionPanelUI : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI factText;
    public Image factBackgroundImage;
    public Sprite factBackground;
    public Button[] answerButtons;

    private Question currentQuestion;
    private QuizManager quizManager;

    #region Button Image
    public Sprite normalSprite;
    public Sprite wrongSprite;
    public Sprite correctSprite;
    #endregion
    #endregion

    #region Methods
    public void Setup(Question q, QuizManager manager)
    {
        currentQuestion = q;
        quizManager = manager;

        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; //Capture index for button
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];

            Image img = answerButtons[i].GetComponent<Image>();
            img.sprite = normalSprite; //The Start Picture

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));

            //Rescale the buttons for next question
            RectTransform rt = answerButtons[i].GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
        }

    }

    void OnAnswerSelected(int index)
    {
        //Deaktivate the buttons
        foreach (var btn in answerButtons)
        {
            btn.interactable = false;
        }

        //Change the sprite on all the buttons and scale them
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image img = answerButtons[i].GetComponent<Image>();
            RectTransform rt = answerButtons[i].GetComponent<RectTransform>();

            bool isCorrect = i == currentQuestion.correctAnswerIndex;
            bool isPlayerChoice = i == index;

            //Show graficly if correct or wrong
            img.sprite = isCorrect ? correctSprite : wrongSprite;

            //Scaling logic
            if (isCorrect || isPlayerChoice) //If it is the correct answer or the player clicked it
            {
                rt.localScale = new Vector3(1.05f, 1.05f, 1f); //Scale op
            }
            else
            {
                //img.sprite = wrongSprite; //Wrong Button
                rt.localScale = new Vector3(0.9f, 0.9f, 1f); //Scale down
            }
        }

        //Show the factbox with delay
        if (!string.IsNullOrEmpty(currentQuestion.factText))
        {
            factText.text = currentQuestion.factText;

            factBackgroundImage.sprite = factBackground;
            StartCoroutine(ShowFactBoxWithDelay(1.0f));

        }

        quizManager.SubmitAnswer(index == currentQuestion.correctAnswerIndex);
    }

    private IEnumerator ShowFactBoxWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        factText.gameObject.SetActive(true);
        factBackgroundImage.gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        factText.gameObject.SetActive(false);
        factBackgroundImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
