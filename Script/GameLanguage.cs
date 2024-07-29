using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLanguage : MonoBehaviour
{

	public static GameLanguage gl;
	public string currentLanguage = "tr";

	Dictionary<string, string> langID;

	// Start is called before the first frame update
	void Start()
	{
		gl = this;






		if (PlayerPrefs.HasKey("GameLanguage"))
		{
			currentLanguage = PlayerPrefs.GetString("GameLanguage");
		}
		else
		{
			ResetLanguage();
		}



		WordDefine();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Setlanguage(string langCode)
	{

		PlayerPrefs.SetString("GameLanguage", langCode);
		currentLanguage = langCode;
	}





	public void ResetLanguage()
	{
		Setlanguage("tr");

	}

	public string Say(string text)
	{

		switch (currentLanguage)
		{
			case "id":
				return FindInDict(langID, text);
			default:
				return text;
		}
	}

	public string FindInDict(Dictionary<string, string> selectedLang, string text)
	{
		if (selectedLang.ContainsKey(text))
			return selectedLang[text];
		else
			return "Untranslated";
	}

	public void WordDefine()
	{

		//Bahasa Indonesia (id)
		// ingilizce
		langID = new Dictionary<string, string>()
		{
			{"OYNA", "PLAY"},
			{"Ba�ar�lar�m", "Achievements"},
			{"Nas�l Oynan�r?", "How To Play?"},
			{"Ayarlar", "Settings"},
			{"M�zik", "Music"},
			{"T�rk�e", "Turkish"},
			{"�leri", "Next"},
			{"Buldu�unuz cevab� buradan yazabilirsiniz.", "You can write the answer you find here."},
			{"Cevab�n�z� buradan onaylayabilirsiniz.", "You can confirm your answer here."},
			{"Buradan ipucu alabilirsiniz.", "You can get tips from here."},
			{"S�radaki kelimeye ge�ebilirsiniz.", "You can move on to the next word."},
			{"�ngilizce", "English"},
			{"�PUCU", "TIPS"},
			{"Kelime Bulmaca", "Word Puzzle"},
			{"Oyun 1 adet ipucu ve bo� kutular ile a��l�r. �pucu ile kelimeyi tahmin edebilir veya daha fazla ipucu almak i�in butona t�klayabilirsin. Ne kadar az ipucu ile bilirsen o kadar y�ksek puan al�rs�n. Her ba�ar�s�z tahminin ile birlikte yeni ipucu a��lmaya devam eder. Son ipucuyu a�t�ktan sonra yanl�� cevap verirsen di�er soruya ge�ersin. B�l�m sonunda 50 veya daha y�ksek puan al�rsan bir sonraki b�l�m a��l�r.", "The game starts with one clue and empty boxes. You can either guess the word using the clue or click the button to get more clues. The fewer clues you use, the higher your score will be. With each incorrect guess, a new clue will be revealed. If you give a wrong answer after revealing the last clue, you will move on to the next question. If you score 50 or more points at the end of the level, the next level will be unlocked."},
			{"Enerjin bitti. S�reyi takip edip enerjinin dolmas�n� bekleyebilir ya da reklam izleyerek enerjini doldurabilirsin!", "Your energy is depleted. You can either wait for the timer to replenish your energy or watch an ad to refill it!"},
			{"Toplam Puan", "Total Score"},
            {"Kelime �pucu", "Word Tips"},
			{"Yaln�zca kelime halinde ipu�lar�", "Tips in words only"},
			{"�pu�lar� c�mle halinde oldu�u i�in bu, daha basit bir oyun modu!", "Since the clues are in sentence form, this is a simpler game mode!"},
			{"Enerjin Bitti!", "Energy is depleted!"},
			{"REKLAM �LE ENERJ� DOLDUR", "REFILL ENERGY WITH AN AD" },
			{"REKLAM �LE 10 �PUCU AL", "REFILL 10 TIPS WITH AN AD" },
			{"Enerjinin yenilenmesi i�in bekleyebilir veya reklam izleyerek enerjini doldurabilirsin!", "You can wait for your energy to replenish or watch an ad to refill it!"},
			{"C�mle �pucu", "Sentence Tips"},
			{"Men�ye D�n", "Back To Menu"},
			{"Genel K�lt�r", "General Knowlegde"},
			{"Klasik genel k�lt�r sorular�yla uzun bir macera", "A long adventure with classic general knowledge questions"},
			{"1. �pucu", "Tip1"},
			{"2. �pucu", "Tip2"},
			{"3. �pucu", "Tip3"},
			{"4. �pucu", "Tip4"},
			{"Matematik", "Maths"},
			{"Hukuk", "Law"},
			{"�lkeler", "Countries"},
			{"Tarih", "History"},
			{"5. �pucu", "Tip5"},
			{"Bonus Soru", "Bonus Game"},
			{"Level:", "Chapter:"},
			{"Bonus soru ile b�l�m� tamamlayabilirsin", "Complete the section with the bonus game"},
			{"Sonraki Kelime", "Next Word"},
			{"�pucu Ver", "Give Tips"},
			{"Onayla", "Confirm"},
			{"Yak�nda..", "Coming Soon.."},
			{"???", "???"},
			{"B�l�m� Tamamlayamad�n", "Chapter Failed"},
			{"B�l�m� Tamamlad�n", "Chapter Complete"},
			{"B�l�mlere D�n", "Back To Chapter"},
			{"Harf Ver", "Give Letter"},
			{"B�l�m� Bitir", "Back To Chapters"},
			{"�pu�lar� Bitti!", "Your Tips Over!"}









		};

	}

}
