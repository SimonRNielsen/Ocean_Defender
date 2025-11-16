using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapScript : MonoBehaviour
{

    private readonly string startScene = "StartMenu";
    private static bool unloadedBootstrapper = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (!unloadedBootstrapper)
        {

            unloadedBootstrapper = true;

            StartCoroutine(InitiateMenu()); //Run last

        }

    }

    // Update is called once per frame
    void Update()
    {



    }

    private IEnumerator InitiateMenu()
    {

        yield return SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);

        yield return SceneManager.UnloadSceneAsync(gameObject.scene);

    }

}
