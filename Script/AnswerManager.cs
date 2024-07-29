using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerManager : MonoBehaviour
{
    public GameObject answerButtonsContainer; // Cevap butonlarýnýn parent GameObject'i
    private List<Button> answerButtons = new List<Button>();
    private int currentAnswerIndex = 0;

    void Start()
    {
        // Cevap butonlarýný dinamik olarak bulup ekleme
        foreach (Transform buttonTransform in answerButtonsContainer.transform)
        {
            Button button = buttonTransform.GetComponent<Button>();
            answerButtons.Add(button);
        }
    }

    public void SetupAnswerButtons(int length)
    {
        currentAnswerIndex = 0;

        // Butonlarýn görünürlüðünü ayarlayýn
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < length)
            {
                answerButtons[i].gameObject.SetActive(true);
                Text buttonText = answerButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = "";
                }
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void AddLetterToAnswer(string letter)
    {
        if (currentAnswerIndex < answerButtons.Count && answerButtons[currentAnswerIndex].gameObject.activeSelf)
        {
            // Mevcut cevaba harfi ekleyin
            Text buttonText = answerButtons[currentAnswerIndex].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = letter;
                Debug.Log("Letter Added: " + letter + " to Button: " + currentAnswerIndex);
                currentAnswerIndex++;
            }
        }
    }

    public void RemoveLastLetterFromAnswer()
    {
        if (currentAnswerIndex > 0)
        {
            currentAnswerIndex--;
            Text buttonText = answerButtons[currentAnswerIndex].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
                Debug.Log("Letter Removed from Button: " + currentAnswerIndex);
            }
        }
    }

    public void ResetAnswer()
    {
        // Cevabý sýfýrlayýn
        foreach (Button button in answerButtons)
        {
            if (button.gameObject.activeSelf)
            {
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = "";
                }
            }
        }
        currentAnswerIndex = 0;
    }

    public string GetUserAnswer()
    {
        string answer = "";
        foreach (Button button in answerButtons)
        {
            if (button.gameObject.activeSelf)
            {
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    answer += buttonText.text;
                }
            }
        }
        return answer;
    }
}
