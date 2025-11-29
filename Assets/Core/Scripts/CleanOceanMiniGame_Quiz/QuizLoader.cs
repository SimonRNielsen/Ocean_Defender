using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    #region Fields
    public string quizSceneName = "Quiz_Stefanie";
    public float quizInterval = 5f;

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
            yield return new WaitForSeconds(quizInterval);

            if (!quizActive)
            {
                StartCoroutine(LoadQuiz());
            }
        }
    }

    IEnumerator LoadQuiz()
    {
        quizActive = true;
        Time.timeScale = 0f;

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
            controller.quizManager.StartQuiz();
            controller.quizManager.quizController = controller;
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



    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
