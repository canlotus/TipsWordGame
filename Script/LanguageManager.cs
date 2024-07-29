using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public LanguageSelection languageSelection; // LanguageSelection scripti referans�

    void Start()
    {
        if (!PlayerPrefs.HasKey("FirstTime"))
        {
            // �lk kez giriyor
            PlayerPrefs.SetInt("FirstTime", 1);

            // Cihaz�n dilini kontrol et
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
