using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintLetterManager : MonoBehaviour
{
    public GameObject hintLetterButtonsContainer; // Harf ipucu butonlarýnýn parent GameObject'i

    private List<Button> hintLetterButtons = new List<Button>();
    private HashSet<int> revealedIndices = new HashSet<int>(); // Açýklanan harflerin indeksleri

    void Start()
    {
        // Butonlarý dinamik olarak oluþtur ve listeye ekle
        foreach (Transform buttonTransform in hintLetterButtonsContainer.transform)
        {
            Button button = buttonTransform.GetComponent<Button>();
            hintLetterButtons.Add(button);
            button.gameObject.SetActive(false); // Baþlangýçta butonlarý gizle
        }
    }

    public void SetupHintLetterButtons(int length)
    {
        // Kelimenin uzunluðuna göre butonlar oluþtur
        for (int i = 0; i < length; i++)
        {
            hintLetterButtons[i].gameObject.SetActive(false); // Baþlangýçta butonlarý gizle
        }
        ResetHintLetters(); // Önceki harf ipuçlarýný sýfýrla
    }

    public void ShowRandomHintLetter(string word)
    {
        if (hintLetterButtons.Count == 0)
        {
            Debug.LogError("Hint letter buttons are not set up.");
            return;
        }

        // Kelimenin harflerinden rastgele bir tanesini seç, daha önce açýklanmamýþ olanlardan
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

        // Harf ipucu butonunu güncelle ve görünür hale getir
        Button button = hintLetterButtons[randomIndex];
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = randomLetter.ToString();
            button.gameObject.SetActive(true); // Butonu görünür yap
        }
        else
        {
            Debug.LogError("Hint letter button does not have a Text component.");
        }

        revealedIndices.Add(randomIndex); // Açýklanan harfi kaydet
    }

    public void ResetHintLetters()
    {
        foreach (var button in hintLetterButtons)
        {
            button.gameObject.SetActive(false); // Butonlarý gizle
        }
        revealedIndices.Clear(); // Açýklanan harfleri sýfýrla
    }
}
