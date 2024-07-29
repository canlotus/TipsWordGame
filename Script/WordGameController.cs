using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;

public class WordGameController : MonoBehaviour
{
    public Text clueText1;
    public Text clueText2;
    public Text clueText3;
    public Text clueText4;
    public Text clueText5;
    public Text questionProgressText; // Sorunun ilerlemesini gösterecek Text bileþeni
    public Text scoreText; // Toplam puaný gösterecek Text bileþeni
    public Text pointFeedbackText; // Puan geri bildirimi gösterecek Text bileþeni
    public GameObject answerManagerObject;
    public Button submitButton; // Onay butonu
    public Button hintButton; // Ýpucu butonu
    public Button nextQuestionButton; // Sýradaki Soru butonu
    public GameObject endPanel; // 10 soru tamamlandýðýnda açýlacak panel
    public Button backToChapterSelectButton; // Chapter Select menüsüne dönecek buton
    public Button backToFirstSceneButton; // FirstScene'e dönmek için buton
    public Button bonusQuestionButton; // Bonus soru butonu
    public GameObject correctImage; // Doðru cevap görseli
    public GameObject incorrectImage; // Yanlýþ cevap görseli
    public Text endPanelScoreText;
    public Text winText; // Kazanma mesajý
    public Text loseText; // Kaybetme mesajý
    public Text chapterText; // Bölüm numarasýný gösterecek Text bileþeni

    private List<string> words = new List<string>();
    private List<List<string>> clues = new List<List<string>>();
    private List<int> selectedWordIndices = new List<int>();
    private int currentWordIndex;
    private int currentClueIndex;
    private int currentQuestionIndex = 0;
    private int totalScore = 0;
    private string selectedChapter;
    private string selectedLanguage;
    private bool isBonusQuestion = false;

    async void Start()
    {
        correctImage.SetActive(false); // Görselleri baþlangýçta gizle
        incorrectImage.SetActive(false);
        pointFeedbackText.gameObject.SetActive(false); // Puan geri bildirimini baþlangýçta gizle
        endPanel.SetActive(false); // End paneli baþlangýçta gizle

        selectedLanguage = PlayerPrefs.GetString("SelectedLanguage", "tr"); // Varsayýlan olarak Türkçe

        await LoadSelectedChapter();
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        clueText4.text = "";
        clueText5.text = "";
        scoreText.text = "Puan: " + totalScore;

        submitButton.onClick.AddListener(OnSubmitButtonClick);
        hintButton.onClick.AddListener(ShowNextClue);
        nextQuestionButton.onClick.AddListener(OnNextQuestionButtonClick); // Sýradaki Soru butonu týklama olayý
        backToChapterSelectButton.onClick.AddListener(OnBackToChapterSelectButtonClick); // Chapter Select menüsüne dönecek buton
        backToFirstSceneButton.onClick.AddListener(OnBackToFirstSceneButtonClick); // FirstScene'e dönecek buton
        bonusQuestionButton.onClick.AddListener(OnBonusQuestionButtonClick); // Bonus soru butonu týklama olayý

        LoadNextQuestion(); // Ýlk soruyu ve ipucunu yüklemek için buraya taþýdýk

        // Bölüm numarasýný göster
        int chapterNumber = int.Parse(selectedChapter.Replace("WordChapter", ""));
        chapterText.text = "" + chapterNumber;
    }

    async Task LoadSelectedChapter()
    {
        selectedChapter = PlayerPrefs.GetString("SelectedChapter", "WordChapter1"); // Varsayýlan olarak WordChapter1
        string jsonContent = await JsonLoader.LoadJsonFromStreamingAssets(selectedChapter + ".json");
        if (jsonContent == null) return;

        ChapterData chapterData = JsonUtility.FromJson<ChapterData>(jsonContent);

        foreach (var item in chapterData.words)
        {
            if (selectedLanguage == "tr")
            {
                words.Add(item.word_tr.ToUpper()); // Doðru cevabý büyük harfe dönüþtürün
                clues.Add(item.clues_tr);
            }
            else
            {
                words.Add(item.word_en.ToUpper()); // Doðru cevabý büyük harfe dönüþtürün
                clues.Add(item.clues_en);
            }
        }

        SelectRandomWords();
    }

    void SelectRandomWords()
    {
        HashSet<int> uniqueIndices = new HashSet<int>();
        while (uniqueIndices.Count < 10)
        {
            int randomIndex = Random.Range(0, words.Count);
            if (!uniqueIndices.Contains(randomIndex))
            {
                uniqueIndices.Add(randomIndex);
            }
        }
        selectedWordIndices = new List<int>(uniqueIndices);
    }

    void LoadNextQuestion()
    {
        if (currentQuestionIndex >= selectedWordIndices.Count)
        {
            Debug.Log("Tüm sorular bitti!");
            SaveChapterScore();
            ShowEndPanel();
            return;
        }

        currentWordIndex = selectedWordIndices[currentQuestionIndex];
        currentClueIndex = 0; // Ýlk ipucuna sýfýrlama
        currentQuestionIndex++;

        // AnswerManager'da harf sayýsý kadar buton oluþturulmasý
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        answerManager.SetupAnswerButtons(words[currentWordIndex].Length);

        // Ýpuçlarýný sýfýrlayýn
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        clueText4.text = "";
        clueText5.text = "";

        // Ýlk ipucunu göster
        ShowNextClue();

        // Sorunun ilerlemesini güncelleyin
        questionProgressText.text = $"{currentQuestionIndex}/{selectedWordIndices.Count}";

        // Ýpucu butonunu yeniden etkinleþtir
        hintButton.interactable = true;
        // Onay butonunu yeniden ayarla
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmitButtonClick);
    }

    void OnSubmitButtonClick()
    {
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        string userAnswer = answerManager.GetUserAnswer().ToUpper(); // Kullanýcý cevabýný büyük harfe dönüþtürün

        if (string.IsNullOrWhiteSpace(userAnswer))
        {
            Debug.Log("Boþ cevap! Lütfen bir þeyler yazýn.");
            return; // Boþ cevabý kabul etmeyin
        }

        if (userAnswer.Equals(words[currentWordIndex]))
        {
            Debug.Log("Doðru cevap!");
            int points = CalculatePoints();
            totalScore += points;
            scoreText.text = "Puan: " + totalScore;
            StartCoroutine(ShowFeedback(true));
            StartCoroutine(ShowPoints(points));
            LoadNextQuestion();
        }
        else
        {
            if (currentClueIndex < 5)
            {
                ShowNextClue();
                answerManager.ResetAnswer(); // Yanlýþ cevap verildiðinde cevaplarý temizle
                StartCoroutine(ShowFeedback(false));
            }
            else
            {
                Debug.Log("Yanlýþ cevap! Bir sonraki soruya geçiliyor.");
                StartCoroutine(ShowFeedback(false));
                StartCoroutine(ShowPoints(0));
                answerManager.ResetAnswer();
                LoadNextQuestion();
            }
        }
    }

    void ShowNextClue()
    {
        if (currentClueIndex == 0)
        {
            clueText1.text = clues[currentWordIndex][currentClueIndex];
        }
        else if (currentClueIndex == 1)
        {
            clueText2.text = clues[currentWordIndex][currentClueIndex];
        }
        else if (currentClueIndex == 2)
        {
            clueText3.text = clues[currentWordIndex][currentClueIndex];
        }
        else if (currentClueIndex == 3)
        {
            clueText4.text = clues[currentWordIndex][currentClueIndex];
        }
        else if (currentClueIndex == 4)
        {
            clueText5.text = clues[currentWordIndex][currentClueIndex];
        }

        currentClueIndex++;

        // Beþinci ipucudan sonra baþka ipucu yok, ipucu butonunu devre dýþý býrak
        if (currentClueIndex >= 5)
        {
            hintButton.interactable = false;
            // Onay butonunu yeniden ayarla
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(OnFinalSubmitButtonClick);
        }
    }

    void OnFinalSubmitButtonClick()
    {
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        string userAnswer = answerManager.GetUserAnswer().ToUpper(); // Kullanýcý cevabýný büyük harfe dönüþtürün

        if (string.IsNullOrWhiteSpace(userAnswer))
        {
            Debug.Log("Boþ cevap! Lütfen bir þeyler yazýn.");
            return; // Boþ cevabý kabul etmeyin
        }

        if (userAnswer.Equals(words[currentWordIndex]))
        {
            Debug.Log("Doðru cevap!");
            int points = CalculatePoints();
            totalScore += points;
            scoreText.text = "Puan: " + totalScore;
            StartCoroutine(ShowFeedback(true));
            StartCoroutine(ShowPoints(points));
            LoadNextQuestion();
        }
        else
        {
            Debug.Log("Yanlýþ cevap! Bir sonraki soruya geçiliyor.");
            StartCoroutine(ShowFeedback(false));
            StartCoroutine(ShowPoints(0));
            answerManager.ResetAnswer(); // Yanlýþ cevap verildiðinde cevaplarý temizle
            LoadNextQuestion();
        }
    }

    void OnNextQuestionButtonClick()
    {
        Debug.Log("Kullanýcý sýradaki soruya geçmek istedi.");
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        answerManager.ResetAnswer(); // Mevcut cevabý temizle
        hintButton.interactable = true; // Ýpucu butonunu yeniden etkinleþtir
        LoadNextQuestion();
    }

    void OnBackToChapterSelectButtonClick()
    {
        SceneManager.LoadScene("WordChapter");
    }

    

    void OnBackToFirstSceneButtonClick()
    {
        SceneManager.LoadScene("WordChapter");
    }

    void OnBonusQuestionButtonClick()
    {
        isBonusQuestion = true;
        endPanel.SetActive(false);
        LoadBonusQuestion();
    }

    async void LoadBonusQuestion()
    {
        string jsonContent = await JsonLoader.LoadJsonFromStreamingAssets("WordChapterBonus.json");
        if (jsonContent == null) return;

        ChapterData bonusData = JsonUtility.FromJson<ChapterData>(jsonContent);

        words.Clear();
        clues.Clear();

        foreach (var item in bonusData.words)
        {
            if (selectedLanguage == "tr")
            {
                words.Add(item.word_tr.ToUpper());
                clues.Add(item.clues_tr);
            }
            else
            {
                words.Add(item.word_en.ToUpper());
                clues.Add(item.clues_en);
            }
        }

        currentWordIndex = Random.Range(0, words.Count);
        currentClueIndex = 0;

        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        answerManager.SetupAnswerButtons(words[currentWordIndex].Length);

        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        clueText4.text = "";
        clueText5.text = "";

        ShowNextClue();

        questionProgressText.text = "Bonus Soru";

        hintButton.interactable = true;
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmitButtonClick);
    }

    void SaveChapterScore()
    {
        int previousScore = PlayerPrefs.GetInt(selectedChapter + "_Score", 0);
        if (totalScore > previousScore)
        {
            PlayerPrefs.SetInt(selectedChapter + "_Score", totalScore);
            PlayerPrefs.Save();
        }

        // Eðer mevcut chapter tamamlandý ve 50 veya daha fazla puan alýndýysa bir sonraki bölümü aç
        int chapterNumber = int.Parse(selectedChapter.Replace("WordChapter", ""));
        if (totalScore >= 50 && chapterNumber < 42) // 42 maksimum chapter numarasý
        {
            PlayerPrefs.SetInt("WordChapter" + (chapterNumber + 1) + "_Unlocked", 1);
            PlayerPrefs.Save();
        }
    }

    void ShowEndPanel()
    {
        endPanel.SetActive(true);
        endPanelScoreText.text = "Toplam Puan: " + totalScore; // End panelde toplam puaný göster

        if (totalScore >= 50)
        {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
            bonusQuestionButton.gameObject.SetActive(false); // 50 puandan yüksekse bonus soru butonunu gizle
        }
        else
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);

            if (!isBonusQuestion)
            {
                bonusQuestionButton.gameObject.SetActive(true);
            }
            else
            {
                bonusQuestionButton.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator ShowFeedback(bool isCorrect)
    {
        if (isCorrect)
        {
            correctImage.SetActive(true);
        }
        else
        {
            incorrectImage.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        correctImage.SetActive(false);
        incorrectImage.SetActive(false);
    }

    IEnumerator ShowPoints(int points)
    {
        pointFeedbackText.gameObject.SetActive(true);
        pointFeedbackText.text = "+" + points.ToString() + " puan";
        yield return new WaitForSeconds(1);
        pointFeedbackText.gameObject.SetActive(false);
    }

    int CalculatePoints()
    {
        if (currentClueIndex == 1) return 10;
        if (currentClueIndex == 2) return 8;
        if (currentClueIndex == 3) return 6;
        if (currentClueIndex == 4) return 4;
        if (currentClueIndex == 5) return 2;
        return 0;
    }

    [System.Serializable]
    public class ChapterData
    {
        public List<WordData> words;
    }

    [System.Serializable]
    public class WordData
    {
        public string word_tr;
        public string word_en;
        public List<string> clues_tr;
        public List<string> clues_en;
    }
}
