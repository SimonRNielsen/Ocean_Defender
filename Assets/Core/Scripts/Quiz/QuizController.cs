using UnityEngine;
using System.Collections;

public class QuizController : MonoBehaviour
{
    #region Fields
    public QuizManager quizManager; //Reference to QuizManager prefab

    //private bool quizActive = false;
    #endregion
    #region Methods
    public void QuizEnded()
    {
        //quizActive = false;
        quizManager.gameObject.SetActive(false);
    }
    #endregion
}
