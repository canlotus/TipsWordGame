using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterButtonManager : MonoBehaviour
{
    public GameObject letterButtonsContainer; // Harf butonlarýnýn parent GameObject'i
    public Button deleteButton; // Sil butonu referansý
    public AnswerManager answerManager; // AnswerManager referansý

    void Start()
    {
        // Harf butonlarýný dinamik olarak bulup ekleme
        foreach (Transform buttonTransform in letterButtonsContainer.transform)
        {
            Button button = buttonTransform.GetComponent<Button>();
            button.onClick.AddListener(() => OnLetterButtonClick(button));
        }

        // Sil butonunun týklama olayýný ekleyin
        deleteButton.onClick.AddListener(OnDeleteButtonClick);
    }

    void OnLetterButtonClick(Button button)
    {
        // Butonun text'ini alýn
        string letter = button.GetComponentInChildren<Text>().text;
        Debug.Log("Button Clicked: " + letter);
        // AnswerManager'da cevaba ekleyin
        answerManager.AddLetterToAnswer(letter);
    }

    void OnDeleteButtonClick()
    {
        Debug.Log("Delete Button Clicked");
        // AnswerManager'da son harfi sil
        answerManager.RemoveLastLetterFromAnswer();
    }
}