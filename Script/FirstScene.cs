using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class FirstScene : MonoBehaviour
{
    public Button MainMenuSelectButton;
    public Button AchievementsButton;
    public Button CloseAchievementsButton;
    public GameObject AchievementsPanel;
    public Text BronzeMedalCountText;
    public Text SilverMedalCountText;
    public Text GoldMedalCountText;
    public Text TotalScoreText;

    // Yeni eklenenler
    public Button HowToPlayButton;
    public Button CloseHowToPlayButton;
    public GameObject HowToPlayPanel;
    public Button OpenEnergyPanelButton;
    public GameObject EnergyPanel;
    public Button CloseEnergyPanelButton;
    public Text TotalEnergyText;
    public Text TotalEnergyTimerText;

    // Intro ve tutorial panelleri
    public GameObject IntroPanel;
    public Button IntroPlayButton;
    public GameObject FirstTimePanel;
    public Button SetTurkishButton;
    public Button SetEnglishButton;
    public Button NextButton1;
    public GameObject TutorialPanel1;
    public Button NextButton2;
    public GameObject TutorialPanel2;
    public Button NextButton3;
    public GameObject TutorialPanel3;
    public Button CloseIntroPanelButton;

    private int maxEnergy = 5;
    private int currentEnergy;
    private TimeSpan energyRechargeTime = TimeSpan.FromMinutes(10);
    private DateTime nextEnergyTime;
    private DateTime lastEnergyTime;

    private void Start()
    {
        //OYUNU YÜKLERKEN ALTTAKÝ 2SÝNÝ SÝL
        //PlayerPrefs.DeleteKey("FirstTime");
        //PlayerPrefs.Save();

        MainMenuSelectButton.onClick.AddListener(PlayGame);
        AchievementsButton.onClick.AddListener(OpenAchievementsPanel);
        CloseAchievementsButton.onClick.AddListener(CloseAchievementsPanel);
        HowToPlayButton.onClick.AddListener(OpenHowToPlayPanel);
        CloseHowToPlayButton.onClick.AddListener(CloseHowToPlayPanel);
        OpenEnergyPanelButton.onClick.AddListener(OpenEnergyPanel);
        CloseEnergyPanelButton.onClick.AddListener(CloseEnergyPanel);

        AchievementsPanel.SetActive(false);
        HowToPlayPanel.SetActive(false);
        EnergyPanel.SetActive(false);

        SetTurkishButton.onClick.AddListener(SetTurkish);
        SetEnglishButton.onClick.AddListener(SetEnglish);
        NextButton1.onClick.AddListener(ShowTutorialPanel1);
        NextButton2.onClick.AddListener(ShowTutorialPanel2);
        NextButton3.onClick.AddListener(ShowTutorialPanel3);
        CloseIntroPanelButton.onClick.AddListener(CloseIntroPanel);
        IntroPlayButton.onClick.AddListener(ShowFirstTimePanel);

        LoadEnergyData();
        UpdateEnergyDisplay();

        InvokeRepeating("UpdateEnergy", 0, 1f);

        if (PlayerPrefs.GetInt("FirstTime", 0) == 0)
        {
            ShowIntroPanel();
        }
        else
        {
            IntroPanel.SetActive(false);
            FirstTimePanel.SetActive(false);
            TutorialPanel1.SetActive(false);
            TutorialPanel2.SetActive(false);
            TutorialPanel3.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenAchievementsPanel()
    {
        UpdateAchievementsData();
        AchievementsPanel.SetActive(true);
    }

    public void CloseAchievementsPanel()
    {
        AchievementsPanel.SetActive(false);
    }

    public void OpenHowToPlayPanel()
    {
        HowToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlayPanel()
    {
        HowToPlayPanel.SetActive(false);
    }

    public void OpenEnergyPanel()
    {
        EnergyPanel.SetActive(true);
    }

    public void CloseEnergyPanel()
    {
        EnergyPanel.SetActive(false);
    }

    private void LoadEnergyData()
    {
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);
        nextEnergyTime = DateTime.Parse(PlayerPrefs.GetString("NextEnergyTime", DateTime.Now.ToString()));
        lastEnergyTime = DateTime.Parse(PlayerPrefs.GetString("LastEnergyTime", DateTime.Now.ToString()));
    }

    private void SaveEnergyData()
    {
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetString("NextEnergyTime", nextEnergyTime.ToString());
        PlayerPrefs.SetString("LastEnergyTime", lastEnergyTime.ToString());
        PlayerPrefs.Save();
    }

    private void UpdateEnergyDisplay()
    {
        TotalEnergyText.text = currentEnergy + "/" + maxEnergy;

        if (currentEnergy < maxEnergy)
        {
            TimeSpan timeToNextEnergy = nextEnergyTime - DateTime.Now;
            if (timeToNextEnergy.TotalSeconds < 0)
            {
                timeToNextEnergy = TimeSpan.Zero;
            }
            TotalEnergyTimerText.text = string.Format("{0:D2}:{1:D2}", timeToNextEnergy.Minutes, timeToNextEnergy.Seconds);
        }
        else
        {
            TotalEnergyTimerText.text = "";
        }
    }

    private void UpdateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = currentTime - lastEnergyTime;
            int energyToReplenish = (int)(timeDifference.TotalMinutes / energyRechargeTime.TotalMinutes);

            if (energyToReplenish > 0)
            {
                currentEnergy = Mathf.Min(currentEnergy + energyToReplenish, maxEnergy);
                lastEnergyTime = currentTime;
                if (currentEnergy < maxEnergy)
                {
                    nextEnergyTime = currentTime.Add(energyRechargeTime);
                }
                SaveEnergyData();
            }
            UpdateEnergyDisplay();
        }
    }

    private void UpdateAchievementsData()
    {
        int totalBronze = 0;
        int totalSilver = 0;
        int totalGold = 0;
        int totalScore = 0;

        // WordChapter madalya ve puanlarýný oku
        for (int i = 1; i <= 27; i++) // 27 chapter için
        {
            string chapterName = "WordChapter" + i;
            int score = PlayerPrefs.GetInt(chapterName + "_Score", 0);
            totalScore += score;

            if (score >= 85)
            {
                totalGold++;
            }
            else if (score >= 66)
            {
                totalSilver++;
            }
            else if (score >= 50)
            {
                totalBronze++;
            }
        }

        // Chapter madalya ve puanlarýný oku
        for (int i = 1; i <= 80; i++) // 27 chapter için
        {
            string chapterName = "Chapter" + i;
            int score = PlayerPrefs.GetInt(chapterName + "_Score", 0);
            totalScore += score;

            if (score >= 85)
            {
                totalGold++;
            }
            else if (score >= 66)
            {
                totalSilver++;
            }
            else if (score >= 50)
            {
                totalBronze++;
            }
        }

        BronzeMedalCountText.text = "" + totalBronze;
        SilverMedalCountText.text = "" + totalSilver;
        GoldMedalCountText.text = "" + totalGold;
        TotalScoreText.text = "" + totalScore;
    }

    private void ShowIntroPanel()
    {
        IntroPanel.SetActive(true);
        FirstTimePanel.SetActive(false);
        TutorialPanel1.SetActive(false);
        TutorialPanel2.SetActive(false);
        TutorialPanel3.SetActive(false);
    }

    private void ShowFirstTimePanel()
    {
        IntroPanel.SetActive(false);
        FirstTimePanel.SetActive(true);
    }

    private void ShowTutorialPanel1()
    {
        FirstTimePanel.SetActive(false);
        TutorialPanel1.SetActive(true);
    }

    private void ShowTutorialPanel2()
    {
        TutorialPanel1.SetActive(false);
        TutorialPanel2.SetActive(true);
    }

    private void ShowTutorialPanel3()
    {
        TutorialPanel2.SetActive(false);
        TutorialPanel3.SetActive(true);
    }

    private void CloseIntroPanel()
    {
        TutorialPanel3.SetActive(false);
        PlayerPrefs.SetInt("FirstTime", 1);
        PlayerPrefs.Save();
    }

    private void SetTurkish()
    {
        PlayerPrefs.SetString("SelectedLanguage", "tr");
        PlayerPrefs.Save();
    }

    private void SetEnglish()
    {
        PlayerPrefs.SetString("SelectedLanguage", "en");
        PlayerPrefs.Save();
    }
}
