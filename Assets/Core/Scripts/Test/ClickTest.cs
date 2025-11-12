using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClickTest : MonoBehaviour, IClickable
{

    private Renderer rend;

    public void OnPrimaryRelease()
    {

        //if (rend != null)
        //    rend.material.color = Color.red;
        StartCoroutine(LoadStartMenu());

    }

    public void OnPrimaryHold(Vector3 movement)
    {

        transform.position += movement;

    }

    public void OnPrimaryClick()
    {

        if (rend != null)
            rend.material.color = Color.green;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rend = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadStartMenu()
    {

        yield return SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Additive);

        yield return SceneManager.UnloadSceneAsync(gameObject.scene);

    }

}
