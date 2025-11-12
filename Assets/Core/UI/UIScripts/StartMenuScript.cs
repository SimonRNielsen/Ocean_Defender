using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuScript : MonoBehaviour
{

    [SerializeField] private Sprite gameLogo, backgroundSprite, closeButtonSprite;
    [SerializeField] private AudioClip buttonClickSound;
    [Header("Button border settings"), SerializeField] private bool enableButtonBorder = false;
    [SerializeField, Range(0, 10)] private float borderSize = 3;
    [SerializeField, Range(0, 255)] private float borderRed = 0, borderGreen = 0, borderBlue = 0, borderOpacity = 0;
    [Space, SerializeField] private List<SceneSelection> scenes;
    private VisualElement header, background, buttonContainer, closeButtonContainer;
    private List<(Button Button, Action CoRoutine)> buttons = new List<(Button Button, Action CoRoutine)>();
    private Color borderColor; //Transparant
    private bool buttonsAdded = false;

    private void Awake()
    {

        borderColor = new Color(borderRed, borderGreen, borderBlue, borderOpacity);

        AssignElements();

        if (FillButtons())
            AddButtonsToMenu();

        SetPictures();

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

        if (buttonClickSound != null)
            DataTransfer_SO.Instance.oneShotSoundEvent?.Invoke(buttonClickSound);

        Time.timeScale = 1f; //Unpause

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        yield return SceneManager.UnloadSceneAsync(gameObject.scene);

    }


    private void SetPictures()
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
            closeButtonContainer = root.Q<VisualElement>("CloseButtonContainer");

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
                ChangeButtonSettings(button, scene.ButtonPicture);

                buttons.Add((button, () => StartCoroutine(LoadMap(scene.Name))));

            }

            return true;

        }

        return false;

    }


    private void AddButtonsToMenu()
    {

        if (buttons != null && buttons.Count > 0 && !buttonsAdded)
        {

            VisualElement container1 = new VisualElement();
            VisualElement container2 = new VisualElement();
            ChangeVisualElementSettings(container1);
            ChangeVisualElementSettings(container2);

            for (int i = 0; i < buttons.Count; i++)
            {

                if (i % 2 == 0)
                    container1.Add(buttons[i].Button);
                else
                    container2.Add(buttons[i].Button);

            }

            buttonContainer.Add(container1);
            if (buttons.Count > 1)
                buttonContainer.Add(container2);

            if (closeButtonSprite != null)
            {

                ChangeVisualElementSettings(closeButtonContainer, false);

                Button closeButton = new Button();
                closeButton.clicked += Quit;
                ChangeButtonSettings(closeButton, closeButtonSprite);
                closeButtonContainer.Add(closeButton);

            }

            buttonsAdded = true;

        }

    }


    private void ChangeVisualElementSettings(VisualElement element, bool inRow = true)
    {

        element.style.flexDirection = inRow ? FlexDirection.Row : FlexDirection.Column;
        element.style.height = inRow ? new Length(50, LengthUnit.Percent) : new Length(100, LengthUnit.Percent);
        element.style.width = new Length(100, LengthUnit.Percent);
        element.style.justifyContent = Justify.Center;
        element.style.alignItems = Align.Center;
        element.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        element.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
        element.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);

    }


    private void ChangeButtonSettings(Button button, Sprite sprite)
    {

        button.style.backgroundImage = new StyleBackground(sprite);
        button.style.width = sprite.rect.width;
        button.style.height = sprite.rect.height;
        button.style.backgroundColor = borderColor;
        //button.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        //button.style.backgroundRepeat = new BackgroundRepeat { x = Repeat.NoRepeat, y = Repeat.NoRepeat };
        if (!enableButtonBorder)
        {

            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderTopWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftColor = borderColor;
            button.style.borderRightColor = borderColor;
            button.style.borderTopColor = borderColor;
            button.style.borderBottomColor = borderColor;

        }
        else
        {

            button.style.borderLeftWidth = borderSize;
            button.style.borderRightWidth = borderSize;
            button.style.borderTopWidth = borderSize;
            button.style.borderBottomWidth = borderSize;
            button.style.borderLeftColor = Color.black;
            button.style.borderRightColor = Color.black;
            button.style.borderTopColor = Color.black;
            button.style.borderBottomColor = Color.black;

        }

    }


    private void Quit()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

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