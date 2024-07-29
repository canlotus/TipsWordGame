using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageSelection : MonoBehaviour
{
    public void SetTurkish()
    {
        PlayerPrefs.SetString("SelectedLanguage", "tr");
        PlayerPrefs.Save();
        
    }

    public void SetEnglish()
    {
        PlayerPrefs.SetString("SelectedLanguage", "en");
        PlayerPrefs.Save();
        
    }
}