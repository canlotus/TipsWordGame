using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
using System; // System.Random kullanmak i�in eklendi

public class GameController : MonoBehaviour
{
    public Text clueText1;
    public Text clueText2;
    public Text clueText3;
    public Text questionProgressText; // Sorunun ilerlemesini g�sterecek Text bile�eni
    public Text scoreText; // Toplam puan� g�sterecek Text bile�eni
    public Text pointFeedbackText; // Puan geri bildirimini ba�lang��ta gizleyecek Text bile�eni
    public Text hintLetterCountText; // �pucu haklar�n� g�sterecek Text bile�eni
    public Text hintLetterTimerText; // �pucu hakk� dolum s�resini g�sterecek Text bile�eni
    public GameObject answerManagerObject;
    public GameObject correctAnswerManagerObject; // Do�ru cevab� g�sterecek AnswerManager objesi
    public GameObject hintLetterManagerObject; // Harf ipucu paneli
    public GameObject endTipsPanel; // �pucu hakk� bitti�inde a��lacak panel
    public Text endTipsTimerText; // �pucu hakk� dolum s�resi
    public Button endTipsCloseButton; // �pucu hakk� bitti�inde paneli kapatma butonu
    public Button endTipsRefillButton; // �pucu hakk� bitti�inde doldurma butonu
    public Text refillButtonTimerText; // Refill butonu �zerinde geri say�m sayac� i�in Text bile�eni
    public Button submitButton; // Onay butonu
    public Button hintButton; // �pucu butonu
    public Button hintLetterButton; // Harf ipucu butonu
    public Button nextQuestionButton; // S�radaki Soru butonu
    public Button backButton; // Her an Chapter Select men�s�ne d�necek buton
    public GameObject endPanel; // 10 soru tamamland���nda a��lacak panel
    public Text endPanelScoreText; // End panelde toplam puan� g�sterecek Text bile�eni
    public Button backToFirstSceneButton; // FirstScene'e d�nmek i�in buton
    public Button bonusQuestionButton; // Bonus soru butonu
    public GameObject correctImage; // Do�ru cevap g�rseli
    public GameObject incorrectImage; // Yanl�� cevap g�rseli
    public Text winText; // Kazanma mesaj�
    public Text loseText; // Kaybetme mesaj�
    public Text bonusMessage; // Bonus mesaj�
    public Text chapterText; // B�l�m numaras�n� g�sterecek Text bile�eni

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
        correctImage.SetActive(false); // G�rselleri ba�lang��ta gizle
        incorrectImage.SetActive(false);
        pointFeedbackText.gameObject.SetActive(false); // Puan geri bildirimini ba�lang��ta gizle
        endPanel.SetActive(false); // End paneli ba�lang��ta gizle
        correctAnswerManagerObject.SetActive(false); // Do�ru cevap panelini ba�lang��ta gizle
        bonusQuestionButton.gameObject.SetActive(false); // Bonus soru butonunu ba�lang��ta gizle
        bonusMessage.gameObject.SetActive(false); // Bonus mesaj�n� ba�lang��ta gizle
        hintLetterManagerObject.SetActive(false); // Harf ipucu panelini ba�lang��ta gizle
        endTipsPanel.SetActive(false); // EndTips panelini ba�lang��ta gizle
        refillButtonTimerText.gameObject.SetActive(false); // Saya� metnini ba�lang��ta gizle

        if (hintLetterManagerObject == null)
        {
            Debug.LogError("HintLetterManagerObject is not assigned in the inspector.");
        }

        selectedLanguage = PlayerPrefs.GetString("SelectedLanguage", "tr"); // Varsay�lan olarak T�rk�e

        await LoadSelectedChapter();
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        scoreText.text = "Puan: " + totalScore;

        submitButton.onClick.AddListener(OnSubmitButtonClick);
        hintButton.onClick.AddListener(ShowNextClue);
        nextQuestionButton.onClick.AddListener(OnNextQuestionButtonClick); // S�radaki Soru butonu t�klama olay�
        backButton.onClick.AddListener(OnBackButtonClick); // Her an Chapter Select men�s�ne d�necek buton
        backToFirstSceneButton.onClick.AddListener(OnBackToFirstSceneButtonClick); // FirstScene'e d�necek buton
        bonusQuestionButton.onClick.AddListener(OnBonusQuestionButtonClick); // Bonus soru butonu t�klama olay�
        hintLetterButton.onClick.AddListener(OnHintLetterButtonClick); // Harf ipucu butonu t�klama olay�
        endTipsCloseButton.onClick.AddListener(CloseEndTipsPanel); // EndTips panelini kapatma butonu
        endTipsRefillButton.onClick.AddListener(OnRefillHintLettersButtonClicked); // EndTips panelindeki harf ipucu doldurma butonu

        LoadNextQuestion(); // �lk soruyu ve ipucunu y�klemek i�in buraya ta��d�k
        LoadHintLetterData();
        LoadRefillButtonData();
        UpdateHintLetterDisplay();
        UpdateRefillButtonTimer();

        // B�l�m numaras�n� g�ster
        int chapterNumber = int.Parse(selectedChapter.Replace("Chapter", ""));
        chapterText.text = "" + chapterNumber;

        InvokeRepeating("UpdateHintLetterTimer", 0, 1f);
        InvokeRepeating("UpdateRefillButtonTimer", 0, 1f);
    }

    async Task LoadSelectedChapter()
    {
        selectedChapter = PlayerPrefs.GetString("SelectedChapter", "Chapter1"); // Varsay�lan olarak Chapter1
        string jsonContent = await JsonLoader.LoadJsonFromStreamingAssets(selectedChapter + ".json");
        if (jsonContent == null) return;

        ChapterData chapterData = JsonUtility.FromJson<ChapterData>(jsonContent);

        foreach (var item in chapterData.words)
        {
            if (selectedLanguage == "tr")
            {
                words.Add(item.word_tr.ToUpper()); // Do�ru cevab� b�y�k harfe d�n��t�r�n
                clues.Add(item.clues_tr);
            }
            else
            {
                words.Add(item.word_en.ToUpper()); // Do�ru cevab� b�y�k harfe d�n��t�r�n
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
            int randomIndex = UnityEngine.Random.Range(0, words.Count); // UnityEngine.Random s�n�f� kullan�l�yor
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
            Debug.Log("T�m sorular bitti!");
            SaveChapterScore();
            ShowEndPanel();
            return;
        }

        currentWordIndex = selectedWordIndices[currentQuestionIndex];
        currentClueIndex = 0; // �lk ipucuna s�f�rlama
        currentQuestionIndex++;

        // AnswerManager'da harf say�s� kadar buton olu�turulmas�
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        answerManager.SetupAnswerButtons(words[currentWordIndex].Length);

        // HintLetterManager'da harf say�s� kadar buton olu�turulmas�
        HintLetterManager hintLetterManager = hintLetterManagerObject.GetComponent<HintLetterManager>();
        hintLetterManager.SetupHintLetterButtons(words[currentWordIndex].Length);

        // �pu�lar�n� s�f�rlay�n
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";

        // �lk ipucunu g�ster
        ShowNextClue();

        // Sorunun ilerlemesini g�ncelleyin
        questionProgressText.text = $"{currentQuestionIndex}/{selectedWordIndices.Count}";

        // �pucu butonunu etkinle�tir
        hintButton.interactable = true;
    }

    void OnSubmitButtonClick()
    {
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        string userAnswer = answerManager.GetUserAnswer().ToUpper(); // Kullan�c� cevab�n� b�y�k harfe d�n��t�r�n

        if (string.IsNullOrWhiteSpace(userAnswer))
        {
            Debug.Log("Bo� cevap! L�tfen bir �eyler yaz�n.");
            return; // Bo� cevab� kabul etmeyin
        }

        if (userAnswer.Equals(words[currentWordIndex]))
        {
            Debug.Log("Do�ru cevap!");
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
                answerManager.ResetAnswer(); // Yanl�� cevap verildi�inde cevaplar� temizle
                StartCoroutine(ShowFeedback(false));
            }
            else
            {
                Debug.Log("Yanl�� cevap! Bir sonraki soruya ge�iliyor.");
                StartCoroutine(ShowCorrectAnswer()); // Do�ru cevab� g�ster
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
            hintLetterManagerObject.SetActive(true); // Hint letter panelini g�r�n�r yap
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

        // ���nc� ipucudan sonra ba�ka ipucu yok, ipucu butonunu devre d��� b�rak
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
        bonusMessage.gameObject.SetActive(false); // Bonus mesaj�n� gizle
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

        // E�er mevcut chapter tamamland� ve 50 veya daha fazla puan al�nd�ysa bir sonraki b�l�m� a�
        int chapterNumber = int.Parse(selectedChapter.Replace("Chapter", ""));
        if (totalScore >= 50 && chapterNumber < 80) // 80 maksimum chapter numaras�
        {
            PlayerPrefs.SetInt("Chapter" + (chapterNumber + 1) + "_Unlocked", 1);
            PlayerPrefs.Save();
        }
    }

    void ShowEndPanel()
    {
        endPanel.SetActive(true);
        endPanelScoreText.text = "" + totalScore; // End panelde toplam puan� g�ster

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
