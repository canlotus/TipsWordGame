using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
using System; // System.Random kullanmak için eklendi

public class GameController : MonoBehaviour
{
    public Text clueText1;
    public Text clueText2;
    public Text clueText3;
    public Text questionProgressText; // Sorunun ilerlemesini gösterecek Text bileþeni
    public Text scoreText; // Toplam puaný gösterecek Text bileþeni
    public Text pointFeedbackText; // Puan geri bildirimini baþlangýçta gizleyecek Text bileþeni
    public Text hintLetterCountText; // Ýpucu haklarýný gösterecek Text bileþeni
    public Text hintLetterTimerText; // Ýpucu hakký dolum süresini gösterecek Text bileþeni
    public GameObject answerManagerObject;
    public GameObject correctAnswerManagerObject; // Doðru cevabý gösterecek AnswerManager objesi
    public GameObject hintLetterManagerObject; // Harf ipucu paneli
    public GameObject endTipsPanel; // Ýpucu hakký bittiðinde açýlacak panel
    public Text endTipsTimerText; // Ýpucu hakký dolum süresi
    public Button endTipsCloseButton; // Ýpucu hakký bittiðinde paneli kapatma butonu
    public Button endTipsRefillButton; // Ýpucu hakký bittiðinde doldurma butonu
    public Text refillButtonTimerText; // Refill butonu üzerinde geri sayým sayacý için Text bileþeni
    public Button submitButton; // Onay butonu
    public Button hintButton; // Ýpucu butonu
    public Button hintLetterButton; // Harf ipucu butonu
    public Button nextQuestionButton; // Sýradaki Soru butonu
    public Button backButton; // Her an Chapter Select menüsüne dönecek buton
    public GameObject endPanel; // 10 soru tamamlandýðýnda açýlacak panel
    public Text endPanelScoreText; // End panelde toplam puaný gösterecek Text bileþeni
    public Button backToFirstSceneButton; // FirstScene'e dönmek için buton
    public Button bonusQuestionButton; // Bonus soru butonu
    public GameObject correctImage; // Doðru cevap görseli
    public GameObject incorrectImage; // Yanlýþ cevap görseli
    public Text winText; // Kazanma mesajý
    public Text loseText; // Kaybetme mesajý
    public Text bonusMessage; // Bonus mesajý
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
    private bool bonusQuestionUsed = false;
    private int maxHintLetters = 20; // Max hint letter count changed to 20
    private int currentHintLetters;
    private TimeSpan hintLetterRechargeTime = TimeSpan.FromHours(12);
    private DateTime nextHintLetterTime;
    private DateTime lastHintLetterTime;

    private bool isRefillButtonDisabled;
    private DateTime refillButtonDisabledUntil;

    async void Start()
    {
        correctImage.SetActive(false); // Görselleri baþlangýçta gizle
        incorrectImage.SetActive(false);
        pointFeedbackText.gameObject.SetActive(false); // Puan geri bildirimini baþlangýçta gizle
        endPanel.SetActive(false); // End paneli baþlangýçta gizle
        correctAnswerManagerObject.SetActive(false); // Doðru cevap panelini baþlangýçta gizle
        bonusQuestionButton.gameObject.SetActive(false); // Bonus soru butonunu baþlangýçta gizle
        bonusMessage.gameObject.SetActive(false); // Bonus mesajýný baþlangýçta gizle
        hintLetterManagerObject.SetActive(false); // Harf ipucu panelini baþlangýçta gizle
        endTipsPanel.SetActive(false); // EndTips panelini baþlangýçta gizle
        refillButtonTimerText.gameObject.SetActive(false); // Sayaç metnini baþlangýçta gizle

        if (hintLetterManagerObject == null)
        {
            Debug.LogError("HintLetterManagerObject is not assigned in the inspector.");
        }

        selectedLanguage = PlayerPrefs.GetString("SelectedLanguage", "tr"); // Varsayýlan olarak Türkçe

        await LoadSelectedChapter();
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        scoreText.text = "Puan: " + totalScore;

        submitButton.onClick.AddListener(OnSubmitButtonClick);
        hintButton.onClick.AddListener(ShowNextClue);
        nextQuestionButton.onClick.AddListener(OnNextQuestionButtonClick); // Sýradaki Soru butonu týklama olayý
        backButton.onClick.AddListener(OnBackButtonClick); // Her an Chapter Select menüsüne dönecek buton
        backToFirstSceneButton.onClick.AddListener(OnBackToFirstSceneButtonClick); // FirstScene'e dönecek buton
        bonusQuestionButton.onClick.AddListener(OnBonusQuestionButtonClick); // Bonus soru butonu týklama olayý
        hintLetterButton.onClick.AddListener(OnHintLetterButtonClick); // Harf ipucu butonu týklama olayý
        endTipsCloseButton.onClick.AddListener(CloseEndTipsPanel); // EndTips panelini kapatma butonu
        endTipsRefillButton.onClick.AddListener(OnRefillHintLettersButtonClicked); // EndTips panelindeki harf ipucu doldurma butonu

        LoadNextQuestion(); // Ýlk soruyu ve ipucunu yüklemek için buraya taþýdýk
        LoadHintLetterData();
        LoadRefillButtonData();
        UpdateHintLetterDisplay();
        UpdateRefillButtonTimer();

        // Bölüm numarasýný göster
        int chapterNumber = int.Parse(selectedChapter.Replace("Chapter", ""));
        chapterText.text = "" + chapterNumber;

        InvokeRepeating("UpdateHintLetterTimer", 0, 1f);
        InvokeRepeating("UpdateRefillButtonTimer", 0, 1f);
    }

    async Task LoadSelectedChapter()
    {
        selectedChapter = PlayerPrefs.GetString("SelectedChapter", "Chapter1"); // Varsayýlan olarak Chapter1
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
            int randomIndex = UnityEngine.Random.Range(0, words.Count); // UnityEngine.Random sýnýfý kullanýlýyor
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

        // HintLetterManager'da harf sayýsý kadar buton oluþturulmasý
        HintLetterManager hintLetterManager = hintLetterManagerObject.GetComponent<HintLetterManager>();
        hintLetterManager.SetupHintLetterButtons(words[currentWordIndex].Length);

        // Ýpuçlarýný sýfýrlayýn
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";

        // Ýlk ipucunu göster
        ShowNextClue();

        // Sorunun ilerlemesini güncelleyin
        questionProgressText.text = $"{currentQuestionIndex}/{selectedWordIndices.Count}";

        // Ýpucu butonunu etkinleþtir
        hintButton.interactable = true;
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
            if (currentClueIndex < 3)
            {
                ShowNextClue();
                answerManager.ResetAnswer(); // Yanlýþ cevap verildiðinde cevaplarý temizle
                StartCoroutine(ShowFeedback(false));
            }
            else
            {
                Debug.Log("Yanlýþ cevap! Bir sonraki soruya geçiliyor.");
                StartCoroutine(ShowCorrectAnswer()); // Doðru cevabý göster
            }
        }
    }

    IEnumerator ShowCorrectAnswer()
    {
        nextQuestionButton.interactable = false; // Disable the button
        CorrectAnswerManager correctAnswerManager = correctAnswerManagerObject.GetComponent<CorrectAnswerManager>();
        correctAnswerManager.SetupCorrectAnswerButtons(words[currentWordIndex]);
        correctAnswerManagerObject.SetActive(true);

        yield return new WaitForSeconds(2);

        correctAnswerManagerObject.SetActive(false);
        nextQuestionButton.interactable = true; // Enable the button again
        LoadNextQuestion();
    }

    void OnNextQuestionButtonClick()
    {
        StartCoroutine(ShowCorrectAnswer());
    }

    void OnHintLetterButtonClick()
    {
        if (currentHintLetters > 0)
        {
            HintLetterManager hintLetterManager = hintLetterManagerObject.GetComponent<HintLetterManager>();
            hintLetterManager.ShowRandomHintLetter(words[currentWordIndex]);
            currentHintLetters--;
            SaveHintLetterData();
            UpdateHintLetterDisplay();
            hintLetterManagerObject.SetActive(true); // Hint letter panelini görünür yap
        }
        else
        {
            endTipsPanel.SetActive(true);
            UpdateEndTipsTimerDisplay();
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

        currentClueIndex++;

        // Üçüncü ipucudan sonra baþka ipucu yok, ipucu butonunu devre dýþý býrak
        if (currentClueIndex >= 3)
        {
            hintButton.interactable = false;
        }
    }

    void OnBackButtonClick()
    {
        SceneManager.LoadScene("ChapterSelect");
    }

    void OnBackToFirstSceneButtonClick()
    {
        SceneManager.LoadScene("ChapterSelect");
    }

    void OnBonusQuestionButtonClick()
    {
        bonusQuestionButton.gameObject.SetActive(false); // Bonus soru butonunu gizle
        bonusMessage.gameObject.SetActive(false); // Bonus mesajýný gizle
        endPanel.SetActive(false); // End paneli gizle

        LoadBonusQuestion();
    }

    async void LoadBonusQuestion()
    {
        string jsonContent = await JsonLoader.LoadJsonFromStreamingAssets("ChapterBonus.json");
        if (jsonContent == null) return;

        ChapterData chapterData = JsonUtility.FromJson<ChapterData>(jsonContent);

        words.Clear();
        clues.Clear();

        foreach (var item in chapterData.words)
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

        SelectRandomBonusQuestion();
        LoadNextQuestion();
    }

    void SelectRandomBonusQuestion()
    {
        int randomIndex = UnityEngine.Random.Range(0, words.Count);
        selectedWordIndices = new List<int> { randomIndex };
        currentQuestionIndex = 0;
        bonusQuestionUsed = true;
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
        int chapterNumber = int.Parse(selectedChapter.Replace("Chapter", ""));
        if (totalScore >= 50 && chapterNumber < 80) // 80 maksimum chapter numarasý
        {
            PlayerPrefs.SetInt("Chapter" + (chapterNumber + 1) + "_Unlocked", 1);
            PlayerPrefs.Save();
        }
    }

    void ShowEndPanel()
    {
        endPanel.SetActive(true);
        endPanelScoreText.text = "" + totalScore; // End panelde toplam puaný göster

        if (totalScore >= 50)
        {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
        }
        else
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);

            if (!bonusQuestionUsed)
            {
                bonusQuestionButton.gameObject.SetActive(true);
                bonusMessage.gameObject.SetActive(true);
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

    void LoadHintLetterData()
    {
        currentHintLetters = PlayerPrefs.GetInt("CurrentHintLetters", maxHintLetters);
        nextHintLetterTime = DateTime.Parse(PlayerPrefs.GetString("NextHintLetterTime", DateTime.Now.ToString()));
        lastHintLetterTime = DateTime.Parse(PlayerPrefs.GetString("LastHintLetterTime", DateTime.Now.ToString()));
    }

    void SaveHintLetterData()
    {
        PlayerPrefs.SetInt("CurrentHintLetters", currentHintLetters);
        PlayerPrefs.SetString("NextHintLetterTime", nextHintLetterTime.ToString());
        PlayerPrefs.SetString("LastHintLetterTime", lastHintLetterTime.ToString());
        PlayerPrefs.Save();
    }

    void UpdateHintLetterDisplay()
    {
        hintLetterCountText.text = $"({currentHintLetters})";

        if (currentHintLetters < maxHintLetters)
        {
            TimeSpan timeToNextHintLetter = nextHintLetterTime - DateTime.Now;
            if (timeToNextHintLetter.TotalSeconds < 0)
            {
                timeToNextHintLetter = TimeSpan.Zero;
            }
            hintLetterTimerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeToNextHintLetter.Hours, timeToNextHintLetter.Minutes, timeToNextHintLetter.Seconds);
        }
        else
        {
            hintLetterTimerText.text = "";
        }
    }

    void UpdateEndTipsTimerDisplay()
    {
        TimeSpan timeToNextHintLetter = nextHintLetterTime - DateTime.Now;
        if (timeToNextHintLetter.TotalSeconds < 0)
        {
            timeToNextHintLetter = TimeSpan.Zero;
        }
        endTipsTimerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeToNextHintLetter.Hours, timeToNextHintLetter.Minutes, timeToNextHintLetter.Seconds);
    }

    void CloseEndTipsPanel()
    {
        endTipsPanel.SetActive(false);
    }

    void RefillHintLetters()
    {
        currentHintLetters = Mathf.Min(currentHintLetters + 10, maxHintLetters); // Increase by 10
        SaveHintLetterData();
        UpdateHintLetterDisplay();
        CloseEndTipsPanel();
    }

    void UpdateHintLetterTimer()
    {
        if (currentHintLetters < maxHintLetters)
        {
            TimeSpan timeToNextHintLetter = nextHintLetterTime - DateTime.Now;
            if (timeToNextHintLetter.TotalSeconds <= 0)
            {
                currentHintLetters = Mathf.Min(currentHintLetters + 5, maxHintLetters); // Increase by 5 every 12 hours
                nextHintLetterTime = DateTime.Now.Add(hintLetterRechargeTime);
                SaveHintLetterData();
                UpdateHintLetterDisplay();
            }
        }
    }

    void LoadRefillButtonData()
    {
        isRefillButtonDisabled = PlayerPrefs.GetInt("IsRefillButtonDisabled", 0) == 1;
        if (isRefillButtonDisabled)
        {
            refillButtonDisabledUntil = DateTime.Parse(PlayerPrefs.GetString("RefillButtonDisabledUntil", DateTime.Now.ToString()));
        }
    }

    void SaveRefillButtonData()
    {
        PlayerPrefs.SetInt("IsRefillButtonDisabled", isRefillButtonDisabled ? 1 : 0);
        PlayerPrefs.SetString("RefillButtonDisabledUntil", refillButtonDisabledUntil.ToString());
        PlayerPrefs.Save();
    }

    void OnRefillHintLettersButtonClicked()
    {
        if (isRefillButtonDisabled)
            return;

        RefillHintLetters();
        DisableRefillButtonForDuration(TimeSpan.FromMinutes(5));
    }

    void DisableRefillButtonForDuration(TimeSpan duration)
    {
        isRefillButtonDisabled = true;
        refillButtonDisabledUntil = DateTime.Now.Add(duration);
        SaveRefillButtonData();
        UpdateRefillButtonTimer();
    }

    void UpdateRefillButtonTimer()
    {
        if (isRefillButtonDisabled)
        {
            TimeSpan timeRemaining = refillButtonDisabledUntil - DateTime.Now;
            if (timeRemaining.TotalSeconds <= 0)
            {
                isRefillButtonDisabled = false;
                refillButtonTimerText.gameObject.SetActive(false);
                endTipsRefillButton.interactable = true;
                SaveRefillButtonData();
            }
            else
            {
                refillButtonTimerText.text = string.Format("{0:D2}:{1:D2}", timeRemaining.Minutes, timeRemaining.Seconds);
                refillButtonTimerText.gameObject.SetActive(true);
                endTipsRefillButton.interactable = false;
            }
        }
        else
        {
            refillButtonTimerText.gameObject.SetActive(false);
            endTipsRefillButton.interactable = true;
        }
    }

    int CalculatePoints()
    {
        if (currentClueIndex == 1) return 10;
        if (currentClueIndex == 2) return 6;
        if (currentClueIndex == 3) return 2;
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
