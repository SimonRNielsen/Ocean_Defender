using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Localization;


public class QuestionPanelUI : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;

    private Question currentQuestion;
    private QuizManager quizManager;

    private bool savedPlayerChoice;
    [SerializeField, Tooltip("Audioclip for correct answear")] private AudioClip correctAudio;
    [SerializeField, Tooltip("Audioclip for wrong answear")] private AudioClip wrongAudio;

    #region Factbox
    public TextMeshProUGUI factText;
    public Button continueButton;
    public Image continueButtonImage;
    #endregion

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

        quizManager.factBoxPanel.SetActive(false);

        //questionText.text = q.questionText;
        q.GetQuestionText(value => questionText.text = value);


        SetupButtons(q);

    }

    private void SetupButtons(Question q)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;

            answerButtons[i].interactable = true;
            //answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];
            //////q.GetAnswerText(i, value =>
            //////{
            //////    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = value;
            //////});
            var btn = answerButtons[i]; // lokal kopi sikrer mod async-fejl

            q.GetAnswerText(i, value =>
            {
                if (btn != null) // sikrer mod UI der skjules
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = value;
            });

            answerButtons[i].GetComponent<Image>().sprite = normalSprite;

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));

            answerButtons[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    void OnAnswerSelected(int index)
    {
        //Deaktivate the buttons
        foreach (var btn in answerButtons)
        {
            btn.interactable = false;
        }

        //Update button graphics
        UpdateButtonGraphics(index);

        //save the players choise
        savedPlayerChoice = (index == currentQuestion.correctAnswerIndex);

        if (savedPlayerChoice)
        {
            DataTransfer_SO.Instance.oneShotSoundEvent?.Invoke(correctAudio);
        }
        else
        {
            DataTransfer_SO.Instance.oneShotSoundEvent?.Invoke(wrongAudio);
        }

        //Show the factbox with delay
        //if (!string.IsNullOrEmpty(currentQuestion.factText))
        //{
        //    StartCoroutine(ShowFactBoxWithDelay(2f));

        //}
        // Localized strings skal altid vises hvis de findes i tabellen
        currentQuestion.GetFactText(value =>
        {
            if (!string.IsNullOrEmpty(value))
                StartCoroutine(ShowFactBoxWithDelay(2f));
        });

    }

    private void UpdateButtonGraphics(int index)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isCorrect = i == currentQuestion.correctAnswerIndex;
            bool isChosen = i == index;

            Image img = answerButtons[i].GetComponent<Image>();
            RectTransform rt = answerButtons[i].GetComponent<RectTransform>();

            img.sprite = isCorrect ? correctSprite : wrongSprite;
            rt.localScale = (isCorrect || isChosen) ? new Vector3(1.05f, 1.05f, 1f) : new Vector3(0.9f, 0.9f, 1f);

        }
    }

    private IEnumerator ShowFactBoxWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        //Hide question and answerbuttons
        questionText.gameObject.SetActive(false);
        foreach (var btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        //Set the text into the factbox
        //quizManager.factText.text = currentQuestion.factText;
        currentQuestion.GetFactText(value =>
        {
            quizManager.factText.text = value;
        });


        //Show the factbox
        quizManager.factBoxPanel.SetActive(true);

        //Handle the continue button via the QuizManager
        quizManager.SetupFactBoxContinueButton(OnContinueClicked);

        //Pause the game while factabox is aktive
        Time.timeScale = 0f;
    }

    private void OnContinueClicked()
    {
        quizManager.factBoxPanel.SetActive(false);

        //Game continues
        Time.timeScale = 1f;

        //Quiz system continues
        quizManager.SubmitAnswer(savedPlayerChoice);
        quizManager.StartCoroutine(quizManager.ProceedAfterFactBox());
    }

    #endregion
}
