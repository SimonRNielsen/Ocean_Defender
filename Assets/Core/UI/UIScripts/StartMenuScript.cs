using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuScript : MonoBehaviour
{

    [Header("Menu visuals and click-sound"), Space, SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private Sprite gameLogo, backgroundSprite, closeButtonSprite;
    [Header("Button border settings"), Space, SerializeField] private bool enableButtonBorder = false;
    [SerializeField, Range(0, 10)] private float borderSize = 3;
    [SerializeField, Range(0, 255)] private float borderRed = 0, borderGreen = 0, borderBlue = 0;
    [SerializeField, Range(0, 1)] private float borderOpacity = 1;
    [Space, SerializeField] private List<SceneSelection> scenes;
    private VisualElement header, background, buttonContainer, settingButtonContainer;
    private Button settingButton;
    private List<(Button Button, Action CoRoutine)> buttons = new List<(Button Button, Action CoRoutine)>();
    private Color borderColor;
    private bool buttonsAdded = false;
    private static float buttonScale = 1f;
    private readonly string timerScene = "RoundTimer";

    //Settings
    private VisualElement settingBox;
    private Slider volumenSlider;
    [SerializeField] private AudioSource audioSource;
    //private VisualElement volumenIcon;
    private Button dansk;
    private Button engelsk;
    private Button tysk;
    private Button exitButton;

    /// <summary>
    /// Set to adjust scaling of buttons (before runtime)
    /// </summary>
    public static float ButtonScale { get => buttonScale; set => buttonScale = value; }


    private void Awake()
    {
        ButtonScale = 0.7f;
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

    /// <summary>
    /// Pauses and subscribes buttons actions to their "clicked" event
    /// </summary>
    private void OnEnable()
    {

        Time.timeScale = 0f; //Pause

        if (buttons != null && buttons.Count != 0)
            foreach (var entry in buttons)
                entry.Button.clicked += entry.CoRoutine;

        if (settingButton != null)
            settingButton.clicked += StartSetting;

        if (exitButton != null)
        {
            exitButton.clicked += CloseSetting;
        }

        if (volumenSlider != null)
        {
            volumenSlider.RegisterValueChangedCallback(OnVolumeChanged);
        }
    }

    /// <summary>
    /// Unsubscribes events from buttons
    /// </summary>
    private void OnDisable()
    {

        if (buttons != null && buttons.Count != 0)
            foreach (var entry in buttons)
                entry.Button.clicked -= entry.CoRoutine;

        if (settingButton != null)
        {
            settingButton.clicked -= StartSetting;
        }

        if (exitButton != null)
        {
            exitButton.clicked -= CloseSetting;
        }

        if (volumenSlider != null)
        {
            volumenSlider.UnregisterValueChangedCallback(OnVolumeChanged);
        }
    }

    /// <summary>
    /// Logic for loading sceneloading
    /// </summary>
    /// <param name="sceneName">Name of scene to load</param>
    /// <returns>Scene loading/unloading</returns>
    private IEnumerator LoadMap(string sceneName)
    {

        if (buttonClickSound != null)
            DataTransfer_SO.Instance.oneShotSoundEvent?.Invoke(buttonClickSound); //Sends audioclip to audio sources and plays it once

        Time.timeScale = 1f; //Unpause

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (sceneName != "Quiz_Stefanie")
            yield return SceneManager.LoadSceneAsync(timerScene, LoadSceneMode.Additive);

        yield return SceneManager.UnloadSceneAsync(gameObject.scene);

    }

    /// <summary>
    /// Assigns pictures for background and header
    /// </summary>
    private void SetPictures()
    {

        if (backgroundSprite != null && background != null)
            background.style.backgroundImage = new StyleBackground(backgroundSprite);

        if (gameLogo != null && header != null)
            header.style.backgroundImage = new StyleBackground(gameLogo);

    }

    /// <summary>
    /// Locates elements that need to be manipulated
    /// </summary>
    private void AssignElements()
    {

        var root = GetComponent<UIDocument>().rootVisualElement;

        if (root != null)
        {

            header = root.Q<VisualElement>("Header");
            background = root.Q<VisualElement>("Background");
            buttonContainer = root.Q<VisualElement>("ButtonContainer");
            settingButtonContainer = root.Q<VisualElement>("SettingButtonContainer");
            settingBox = root.Q<VisualElement>("SettingBox");
            volumenSlider = (Slider)root.Q<VisualElement>("VolumenSlider");
            //volumenIcon = root.Q<VisualElement>("VolumIkon");
            exitButton = (Button)root.Q<VisualElement>("ExitButton");
        }

    }

    /// <summary>
    /// Translates info from editor and converts them into buttons and actions
    /// </summary>
    /// <returns>True if operation successful</returns>
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

    /// <summary>
    /// Adds buttons to 1-2 visual elements, and adds close button at the bottom
    /// </summary>
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

                ChangeVisualElementSettings(settingButtonContainer, false);

                settingButton = new Button();
                ChangeButtonSettings(settingButton, closeButtonSprite);
                settingButtonContainer.Add(settingButton);

            }

            buttonsAdded = true;

        }

    }

    /// <summary>
    /// Sets up a VisualElement to specifications
    /// </summary>
    /// <param name="element">Element to be changed</param>
    /// <param name="inRow">Set false if buttons shouldn't be organized in a row</param>
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

    /// <summary>
    /// Sets up a Button to specifications
    /// </summary>
    /// <param name="button">Button to be changed</param>
    /// <param name="sprite">Sprite to assign to button</param>
    private void ChangeButtonSettings(Button button, Sprite sprite)
    {


        button.style.backgroundImage = new StyleBackground(sprite);
        button.style.width = sprite.rect.width * buttonScale;
        button.style.height = sprite.rect.height * buttonScale;
        button.style.backgroundColor = new Color(0, 0, 0, 0);
        button.style.borderLeftColor = borderColor;
        button.style.borderRightColor = borderColor;
        button.style.borderTopColor = borderColor;
        button.style.borderBottomColor = borderColor;

        if (!enableButtonBorder)
        {

            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderTopWidth = 0;
            button.style.borderBottomWidth = 0;

        }
        else
        {

            button.style.borderLeftWidth = borderSize;
            button.style.borderRightWidth = borderSize;
            button.style.borderTopWidth = borderSize;
            button.style.borderBottomWidth = borderSize;

        }

    }

    /// <summary>
    /// Program shutdown method
    /// </summary>
    private void Quit()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }


    private void StartSetting()
    {
        Debug.LogWarning("Setting");

        //Showing settingBox 
        settingBox.SetEnabled(true);
        settingBox.style.display = DisplayStyle.Flex;


        //Shutting Visuelelements and setting button down
        header.SetEnabled(false);
        header.style.display = DisplayStyle.None;
        buttonContainer.SetEnabled(false);
        buttonContainer.style.display = DisplayStyle.None;
        settingButton.SetEnabled(false);
        settingButton.style.display = DisplayStyle.None;
    }

    private void CloseSetting()
    {
        Debug.LogWarning("Setting off");

        //Enable settingBox 
        settingBox.SetEnabled(false);
        settingBox.style.display = DisplayStyle.None;


        //Unenable
        header.SetEnabled(true);
        header.style.display = DisplayStyle.Flex;
        buttonContainer.SetEnabled(true);
        buttonContainer.style.display = DisplayStyle.Flex;
        settingButton.SetEnabled(true);
        settingButton.style.display = DisplayStyle.Flex;
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        if (audioSource != null)
        {
            audioSource.volume = evt.newValue;
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