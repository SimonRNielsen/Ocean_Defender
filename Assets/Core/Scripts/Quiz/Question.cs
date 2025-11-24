using UnityEngine;

[CreateAssetMenu(menuName = "Quiz/Question")]
public class Question : ScriptableObject
{
    [TextArea]
    public string questionText;

    public string[] answers; //4 possible answers
    public int correctAnswerIndex; //Index for the correct answer
}