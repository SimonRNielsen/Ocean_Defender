using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Quiz/Question")]
public partial class Question : ScriptableObject
{
    //    [TextArea]
    //    public string questionText;

    //    public string[] answers; //4 possible answers
    //    public int correctAnswerIndex; //Index for the correct answer

    //    [TextArea]
    //    public string factText; //Factbox after you have answered a question

    public LocalizedString questionText;

    public LocalizedString[] answers; // 4 localized answers

    public int correctAnswerIndex;

    public LocalizedString factText;
}