using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public LanguageSelection languageSelection; // LanguageSelection scripti referansý

    void Start()
    {
        if (!PlayerPrefs.HasKey("FirstTime"))
        {
            // Ýlk kez giriyor
            PlayerPrefs.SetInt("FirstTime", 1);

            // Cihazýn dilini kontrol et
            if (Application.systemLanguage == SystemLanguage.Turkish)
            {
                languageSelection.SetTurkish();
            }
            else
            {
                languageSelection.SetEnglish();
            }

            PlayerPrefs.Save();
        }
    }
}
