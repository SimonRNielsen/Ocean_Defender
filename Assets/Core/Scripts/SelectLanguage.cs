//using TMPro;
//using UnityEngine;
//using UnityEngine.UIElements;
//using static SelectLanguage;
//using UnityEngine.Localization.Settings;
//using UnityEngine.Localization;

//public class SelectLanguage : MonoBehaviour
//{
//    //public enum TextKey
//    //{
//    //    StartButton,
//    //    QuitButton,
//    //    GameOver,
//    //    Pause,
//    //    Resume,
//    //    // ...og hvad du ellers skal bruge
//    //}

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

  

   
//}


////[RequireComponent(typeof(TMP_Text))]
////public class LocalizedTMP : MonoBehaviour
////{
////    public TextKey key;
////    TMP_Text txt;

////    void Awake()
////    {
////        txt = GetComponent<TMP_Text>();
////        UpdateText();
////    }

////    void OnEnable()
////    {
////        LanguageManager.Instance.OnLanguageChanged += UpdateText;
////    }

////    void OnDisable()
////    {
////        LanguageManager.Instance.OnLanguageChanged -= UpdateText;
////    }

////    void UpdateText()
////    {
////        txt.text = LanguageManager.Instance.Get(key);
////    }
////}




////public class LocalizedUIElement
////{
////    private TextElement element;
////    private TextKey key;

////    public LocalizedUIElement(TextElement element, TextKey key)
////    {
////        this.element = element;
////        this.key = key;

////        UpdateText();
////        LanguageManager.Instance.OnLanguageChanged += UpdateText;
////    }

////    private void UpdateText()
////    {
////        element.text = LanguageManager.Instance.Get(key);
////    }
////}

