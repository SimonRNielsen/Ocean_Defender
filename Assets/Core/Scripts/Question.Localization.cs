// Question.Localization.cs
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class Question
{
    // Henter spørgsmålet én gang og returnerer resultatet via callback
    public void GetQuestionText(Action<string> callback)
    {
        if (questionText == null)
        {
            callback?.Invoke(string.Empty);
            return;
        }

        AsyncOperationHandle<string> handle = questionText.GetLocalizedStringAsync();
        handle.Completed += (op) =>
        {
            callback?.Invoke(op.Status == AsyncOperationStatus.Succeeded ? op.Result : string.Empty);
            // frigiv handle for ikke at lække
            Addressables.Release(op);
        };
    }

    // Henter et svar (index) én gang og returnerer via callback
    public void GetAnswerText(int index, Action<string> callback)
    {
        if (answers == null || index < 0 || index >= answers.Length || answers[index] == null)
        {
            callback?.Invoke(string.Empty);
            return;
        }

        AsyncOperationHandle<string> handle = answers[index].GetLocalizedStringAsync();
        handle.Completed += (op) =>
        {
            callback?.Invoke(op.Status == AsyncOperationStatus.Succeeded ? op.Result : string.Empty);
            Addressables.Release(op);
        };
    }

    // Henter fact-teksten én gang og returnerer via callback
    public void GetFactText(Action<string> callback)
    {
        if (factText == null)
        {
            callback?.Invoke(string.Empty);
            return;
        }

        AsyncOperationHandle<string> handle = factText.GetLocalizedStringAsync();
        handle.Completed += (op) =>
        {
            callback?.Invoke(op.Status == AsyncOperationStatus.Succeeded ? op.Result : string.Empty);
            Addressables.Release(op);
        };
    }
}
