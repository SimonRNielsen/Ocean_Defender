using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuScript : MonoBehaviour
{

    [SerializeField] private Sprite gameLogo;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private List<SceneSelection> scenes;
    [SerializeField] private AudioClip buttonClickSound;
    private VisualElement header, background, buttonContainer;
    private List<(Button Button, Action CoRoutine)> buttons = new List<(Button Button, Action CoRoutine)>();

    private void Awake()
    {

        AssignElements();

        if (FillButtons())
            AddButtonsToMenu();

        ChangePictures();

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }


    // Update is called once per frame
    void Update()
    {



    }


    private void OnEnable()
    {

        Time.timeScale = 0f; //Pause

        if (buttons != null && buttons.Count != 0)
            foreach (var entry in buttons)
                entry.Button.clicked += entry.CoRoutine;

    }


    private void OnDisable()
    {

        if (buttons != null && buttons.Count != 0)
            foreach (var entry in buttons)
                entry.Button.clicked -= entry.CoRoutine;

    }


    private IEnumerator LoadMap(string sceneName)
    {

        //ButtonClickSound event here

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Time.timeScale = 1f; //Unpause

        yield return SceneManager.UnloadSceneAsync(gameObject.scene);

    }


    private void ChangePictures()
    {

        if (backgroundSprite != null && background != null)
            background.style.backgroundImage = new StyleBackground(backgroundSprite);

        if (gameLogo != null && header != null)
            header.style.backgroundImage = new StyleBackground(gameLogo);

    }


    private void AssignElements()
    {

        var root = GetComponent<UIDocument>().rootVisualElement;

        if (root != null)
        {

            header = root.Q<VisualElement>("Header");
            background = root.Q<VisualElement>("Background");
            buttonContainer = root.Q<VisualElement>("ButtonContainer");

        }

    }


    private bool FillButtons()
    {

        if (scenes != null && scenes.Count > 0)
        {

            foreach (SceneSelection scene in scenes)
            {

                if (string.IsNullOrWhiteSpace(scene.Name) || scene.ButtonPicture == null) continue; //Don't add incomplete entries

                Button button = new Button();
                button.style.backgroundImage = new StyleBackground(scene.ButtonPicture);
                buttons.Add((button, () => StartCoroutine(LoadMap(scene.Name))));

            }

            return true;

        }

        return false;

    }


    private void AddButtonsToMenu()
    {

        if (buttons != null && buttons.Count > 0)
            for (int i = 0; i < buttons.Count; i++)
            {

                buttonContainer.Add(buttons[i].Button);

            }

    }

}


[Serializable]
public class SceneSelection
{

    [Header("Scene info"), Space]
    [SerializeField, Tooltip("String with name of scene")] private string sceneName;
    [SerializeField, Tooltip("Picture to display button as")] private Sprite buttonPicture;

    public string Name { get => sceneName; }
    public Sprite ButtonPicture { get => buttonPicture; }

}