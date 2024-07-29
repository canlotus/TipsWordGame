using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterButtonManager : MonoBehaviour
{
    public GameObject letterButtonsContainer; // Harf butonlar�n�n parent GameObject'i
    public Button deleteButton; // Sil butonu referans�
    public AnswerManager answerManager; // AnswerManager referans�

    void Start()
    {
        // Harf butonlar�n� dinamik olarak bulup ekleme
        foreach (Transform buttonTransform in letterButtonsContainer.transform)
        {
            Button button = buttonTransform.GetComponent<Button>();
            button.onClick.AddListener(() => OnLetterButtonClick(button));
        }

        // Sil butonunun t�klama olay�n� ekleyin
        deleteButton.onClick.AddListener(OnDeleteButtonClick);
    }

    void OnLetterButtonClick(Button button)
    {
        // Butonun text'ini al�n
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