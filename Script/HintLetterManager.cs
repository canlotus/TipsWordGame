using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintLetterManager : MonoBehaviour
{
    public GameObject hintLetterButtonsContainer; // Harf ipucu butonlar�n�n parent GameObject'i

    private List<Button> hintLetterButtons = new List<Button>();
    private HashSet<int> revealedIndices = new HashSet<int>(); // A��klanan harflerin indeksleri

    void Start()
    {
        // Butonlar� dinamik olarak olu�tur ve listeye ekle
        foreach (Transform buttonTransform in hintLetterButtonsContainer.transform)
        {
            Button button = buttonTransform.GetComponent<Button>();
            hintLetterButtons.Add(button);
            button.gameObject.SetActive(false); // Ba�lang��ta butonlar� gizle
        }
    }

    public void SetupHintLetterButtons(int length)
    {
        // Kelimenin uzunlu�una g�re butonlar olu�tur
        for (int i = 0; i < length; i++)
        {
            hintLetterButtons[i].gameObject.SetActive(false); // Ba�lang��ta butonlar� gizle
        }
        ResetHintLetters(); // �nceki harf ipu�lar�n� s�f�rla
    }

    public void ShowRandomHintLetter(string word)
    {
        if (hintLetterButtons.Count == 0)
        {
            Debug.LogError("Hint letter buttons are not set up.");
            return;
        }

        // Kelimenin harflerinden rastgele bir tanesini se�, daha �nce a��klanmam�� olanlardan
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < word.Length; i++)
        {
            if (!revealedIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0)
        {
            Debug.Log("All letters have been revealed.");
            return;
        }

        int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
        char randomLetter = word[randomIndex];

        // Harf ipucu butonunu g�ncelle ve g�r�n�r hale getir
        Button button = hintLetterButtons[randomIndex];
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = randomLetter.ToString();
            button.gameObject.SetActive(true); // Butonu g�r�n�r yap
        }
        else
        {
            Debug.LogError("Hint letter button does not have a Text component.");
        }

        revealedIndices.Add(randomIndex); // A��klanan harfi kaydet
    }

    public void ResetHintLetters()
    {
        foreach (var button in hintLetterButtons)
        {
            button.gameObject.SetActive(false); // Butonlar� gizle
        }
        revealedIndices.Clear(); // A��klanan harfleri s�f�rla
    }
}
