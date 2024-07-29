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
    public Text questionProgressText; // Sorunun ilerlemesini g�sterecek Text bile�eni
    public Text scoreText; // Toplam puan� g�sterecek Text bile�eni
    public Text pointFeedbackText; // Puan geri bildirimi g�sterecek Text bile�eni
    public GameObject answerManagerObject;
    public Button submitButton; // Onay butonu
    public Button hintButton; // �pucu butonu
    public Button nextQuestionButton; // S�radaki Soru butonu
    public GameObject endPanel; // 10 soru tamamland���nda a��lacak panel
    public Button backToChapterSelectButton; // Chapter Select men�s�ne d�necek buton
    public Button backToFirstSceneButton; // FirstScene'e d�nmek i�in buton
    public Button bonusQuestionButton; // Bonus soru butonu
    public GameObject correctImage; // Do�ru cevap g�rseli
    public GameObject incorrectImage; // Yanl�� cevap g�rseli
    public Text endPanelScoreText;
    public Text winText; // Kazanma mesaj�
    public Text loseText; // Kaybetme mesaj�
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
    private bool isBonusQuestion = false;

    async void Start()
    {
        correctImage.SetActive(false); // G�rselleri ba�lang��ta gizle
        incorrectImage.SetActive(false);
        pointFeedbackText.gameObject.SetActive(false); // Puan geri bildirimini ba�lang��ta gizle
        endPanel.SetActive(false); // End paneli ba�lang��ta gizle

        selectedLanguage = PlayerPrefs.GetString("SelectedLanguage", "tr"); // Varsay�lan olarak T�rk�e

        await LoadSelectedChapter();
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        clueText4.text = "";
        clueText5.text = "";
        scoreText.text = "Puan: " + totalScore;

        submitButton.onClick.AddListener(OnSubmitButtonClick);
        hintButton.onClick.AddListener(ShowNextClue);
        nextQuestionButton.onClick.AddListener(OnNextQuestionButtonClick); // S�radaki Soru butonu t�klama olay�
        backToChapterSelectButton.onClick.AddListener(OnBackToChapterSelectButtonClick); // Chapter Select men�s�ne d�necek buton
        backToFirstSceneButton.onClick.AddListener(OnBackToFirstSceneButtonClick); // FirstScene'e d�necek buton
        bonusQuestionButton.onClick.AddListener(OnBonusQuestionButtonClick); // Bonus soru butonu t�klama olay�

        LoadNextQuestion(); // �lk soruyu ve ipucunu y�klemek i�in buraya ta��d�k

        // B�l�m numaras�n� g�ster
        int chapterNumber = int.Parse(selectedChapter.Replace("WordChapter", ""));
        chapterText.text = "" + chapterNumber;
    }

    async Task LoadSelectedChapter()
    {
        selectedChapter = PlayerPrefs.GetString("SelectedChapter", "WordChapter1"); // Varsay�lan olarak WordChapter1
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

        // �pu�lar�n� s�f�rlay�n
        clueText1.text = "";
        clueText2.text = "";
        clueText3.text = "";
        clueText4.text = "";
        clueText5.text = "";

        // �lk ipucunu g�ster
        ShowNextClue();

        // Sorunun ilerlemesini g�ncelleyin
        questionProgressText.text = $"{currentQuestionIndex}/{selectedWordIndices.Count}";

        // �pucu butonunu yeniden etkinle�tir
        hintButton.interactable = true;
        // Onay butonunu yeniden ayarla
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmitButtonClick);
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
            if (currentClueIndex < 5)
            {
                ShowNextClue();
                answerManager.ResetAnswer(); // Yanl�� cevap verildi�inde cevaplar� temizle
                StartCoroutine(ShowFeedback(false));
            }
            else
            {
                Debug.Log("Yanl�� cevap! Bir sonraki soruya ge�iliyor.");
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

        // Be�inci ipucudan sonra ba�ka ipucu yok, ipucu butonunu devre d��� b�rak
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
            Debug.Log("Yanl�� cevap! Bir sonraki soruya ge�iliyor.");
            StartCoroutine(ShowFeedback(false));
            StartCoroutine(ShowPoints(0));
            answerManager.ResetAnswer(); // Yanl�� cevap verildi�inde cevaplar� temizle
            LoadNextQuestion();
        }
    }

    void OnNextQuestionButtonClick()
    {
        Debug.Log("Kullan�c� s�radaki soruya ge�mek istedi.");
        AnswerManager answerManager = answerManagerObject.GetComponent<AnswerManager>();
        answerManager.ResetAnswer(); // Mevcut cevab� temizle
        hintButton.interactable = true; // �pucu butonunu yeniden etkinle�tir
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

        // E�er mevcut chapter tamamland� ve 50 veya daha fazla puan al�nd�ysa bir sonraki b�l�m� a�
        int chapterNumber = int.Parse(selectedChapter.Replace("WordChapter", ""));
        if (totalScore >= 50 && chapterNumber < 42) // 42 maksimum chapter numaras�
        {
            PlayerPrefs.SetInt("WordChapter" + (chapterNumber + 1) + "_Unlocked", 1);
            PlayerPrefs.Save();
        }
    }

    void ShowEndPanel()
    {
        endPanel.SetActive(true);
        endPanelScoreText.text = "Toplam Puan: " + totalScore; // End panelde toplam puan� g�ster

        if (totalScore >= 50)
        {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
            bonusQuestionButton.gameObject.SetActive(false); // 50 puandan y�ksekse bonus soru butonunu gizle
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
