using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorrectAnswerManager : MonoBehaviour
{
    public GameObject correctAnswerButtonsContainer; // Do�ru cevab�n butonlar�n�n parent GameObject'i
    private List<Button> correctAnswerButtons = new List<Button>();

    void Start()
    {
        // Do�ru cevap butonlar�n� dinamik olarak bulup ekleme
        foreach (Transform buttonTransform in correctAnswerButtonsContainer.transform)
        {
            Button button = buttonTransform.GetComponent<Button>();
            correctAnswerButtons.Add(button);
        }
    }

    public void SetupCorrectAnswerButtons(string correctAnswer)
    {
        // Butonlar�n g�r�n�rl���n� ayarlay�n
        for (int i = 0; i < correctAnswerButtons.Count; i++)
        {
            if (i < correctAnswer.Length)
            {
                correctAnswerButtons[i].gameObject.SetActive(true);
                Text buttonText = correctAnswerButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = correctAnswer[i].ToString();
                }
            }
            else
            {
                correctAnswerButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
