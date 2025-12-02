using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    #region Fields
    public string quizSceneName = "Quiz_Stefanie";
    public float quizInterval = 5f;

    public QuestionList questionListToUse;

    private bool quizActive = false;
    #endregion

    #region Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(QuizTimer());
    }

    IEnumerator QuizTimer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(quizInterval);

            if (!quizActive && !SceneManager.GetSceneByName(quizSceneName).isLoaded)
            {
                StartCoroutine(LoadQuiz());
            }
        }
    }

    IEnumerator LoadQuiz()
    {
        quizActive = true;

        //Load quiz scene on top (Additive)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(quizSceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //FInd quizcontroller in the loaded scene
        Scene quizScene = SceneManager.GetSceneByName(quizSceneName);
        GameObject[] rootObjects = quizScene.GetRootGameObjects();
        QuizController controller = null;

        foreach (var obj in rootObjects)
        {
            controller = obj.GetComponentInChildren<QuizController>();
            if (controller != null)
            {
                break;
            }
        }

        if (controller != null)
        {
            controller.quizManager.quizController = controller; //Set the controller
            controller.quizManager.quizData = questionListToUse; //Set the question list
            controller.quizManager.StartQuiz(); //Start quiz
        }
        else
        {
            Debug.LogError("QuizController not found on loaded scene");
        }

        //Wait till quiz is finished
        while (controller != null && controller.gameObject.activeSelf)
        {
            yield return null;
        }

        //Unload quiz scene 
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(quizSceneName);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        Time.timeScale = 1f;
        quizActive = false;
    }
    #endregion
}
