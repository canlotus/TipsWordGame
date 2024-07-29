using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button ChapterSelectButton;
    public Button WordChapterButton;
    public Button BackToFirstSceneButton; // Yeni eklenen buton
    public Button UnclickableButton1; // ��levsiz buton 1
    public Button UnclickableButton2; // ��levsiz buton 2
    public Button UnclickableButton3; // ��levsiz buton 3
    public Button UnclickableButton4; // ��levsiz buton 4
    public Button UnclickableButton5; // ��levsiz buton 5
    public Button UnclickableButton6; // ��levsiz buton 6
    public Button UnclickableButton7; // ��levsiz buton 7
    public Button UnclickableButton8; // ��levsiz buton 8

    private void Start()
    {
        ChapterSelectButton.onClick.AddListener(PlayGame);
        WordChapterButton.onClick.AddListener(WordChapterSelectButton);
        BackToFirstSceneButton.onClick.AddListener(BackToFirstScene); // Yeni buton i�in dinleyici

        // ��levsiz butonlar� t�klanamaz yap
        UnclickableButton1.interactable = false;
        UnclickableButton2.interactable = false;
        UnclickableButton3.interactable = false;
        UnclickableButton4.interactable = false;
        UnclickableButton5.interactable = false;
        UnclickableButton6.interactable = false;
        UnclickableButton7.interactable = false;
        UnclickableButton8.interactable = false;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("ChapterSelect");
    }

    public void WordChapterSelectButton()
    {
        SceneManager.LoadScene("WordChapter");
    }

    public void BackToFirstScene() // Yeni fonksiyon
    {
        SceneManager.LoadScene("FirstScene");
    }
}
