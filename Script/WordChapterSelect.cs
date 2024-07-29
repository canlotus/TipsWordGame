using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class WordChapterSelect : MonoBehaviour
{
    public Button[] chapterButtons;
    public Text[] chapterScoreTexts;
    public GameObject[] bronzeMedals;
    public GameObject[] silverMedals;
    public GameObject[] goldMedals;
    public Button backButton;
    public Text bronzeMedalsCount;
    public Text silverMedalsCount;
    public Text goldMedalsCount;
    public Text totalScoreText;

    // Enerji ile ilgili
    public Text energyText;
    public Text energyTimerText;
    public GameObject outOfEnergyPanel;
    public Text outOfEnergyTimerText;
    public Button closeOutOfEnergyPanelButton;

    private int maxEnergy = 5;
    private int currentEnergy;
    private TimeSpan energyRechargeTime = TimeSpan.FromMinutes(10);
    private DateTime nextEnergyTime;
    private DateTime lastEnergyTime;

    private void Start()
    {
        LoadEnergyData();
        UpdateEnergyDisplay();
        outOfEnergyPanel.SetActive(false);

        closeOutOfEnergyPanelButton.onClick.AddListener(CloseOutOfEnergyPanel);
        backButton.onClick.AddListener(OnBackButtonClick);

        int totalBronzeMedals = 0;
        int totalSilverMedals = 0;
        int totalGoldMedals = 0;
        int totalScore = 0;

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            int chapterNumber = i + 1;
            string chapterName = "WordChapter" + chapterNumber;

            int index = i;
            chapterButtons[i].onClick.AddListener(() => OnChapterButtonClick(chapterName));

            bool isUnlocked = PlayerPrefs.GetInt(chapterName + "_Unlocked", chapterNumber == 1 ? 1 : 0) == 1;
            chapterButtons[i].interactable = isUnlocked;

            int chapterScore = PlayerPrefs.GetInt(chapterName + "_Score", 0);
            chapterScoreTexts[i].text = chapterScore + "/100";
            totalScore += chapterScore;

            bronzeMedals[i].SetActive(false);
            silverMedals[i].SetActive(false);
            goldMedals[i].SetActive(false);

            if (chapterScore >= 85)
            {
                goldMedals[i].SetActive(true);
                totalGoldMedals++;
            }
            else if (chapterScore >= 66)
            {
                silverMedals[i].SetActive(true);
                totalSilverMedals++;
            }
            else if (chapterScore >= 50)
            {
                bronzeMedals[i].SetActive(true);
                totalBronzeMedals++;
            }
        }

        bronzeMedalsCount.text = "" + totalBronzeMedals;
        silverMedalsCount.text = "" + totalSilverMedals;
        goldMedalsCount.text = "" + totalGoldMedals;
        totalScoreText.text = "" + totalScore;

        InvokeRepeating("UpdateEnergy", 0, 1f);
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
        energyText.text = currentEnergy + "/" + maxEnergy;

        if (currentEnergy < maxEnergy)
        {
            TimeSpan timeToNextEnergy = nextEnergyTime - DateTime.Now;
            if (timeToNextEnergy.TotalSeconds < 0)
            {
                timeToNextEnergy = TimeSpan.Zero;
            }
            energyTimerText.text = string.Format("{0:D2}:{1:D2}", timeToNextEnergy.Minutes, timeToNextEnergy.Seconds);
            outOfEnergyTimerText.text = energyTimerText.text;
        }
        else
        {
            energyTimerText.text = "";
            outOfEnergyTimerText.text = "";
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

    public void OnChapterButtonClick(string chapterName)
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;
            if (currentEnergy == maxEnergy - 1)
            {
                nextEnergyTime = DateTime.Now.Add(energyRechargeTime);
            }
            SaveEnergyData();
            UpdateEnergyDisplay();
            PlayerPrefs.SetString("SelectedChapter", chapterName);
            SceneManager.LoadScene("WordGameScene");
        }
        else
        {
            outOfEnergyPanel.SetActive(true);
        }
    }

    private void CloseOutOfEnergyPanel()
    {
        outOfEnergyPanel.SetActive(false);
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("FirstScene");
    }
}
