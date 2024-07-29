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
			{"Baþarýlarým", "Achievements"},
			{"Nasýl Oynanýr?", "How To Play?"},
			{"Ayarlar", "Settings"},
			{"Müzik", "Music"},
			{"Türkçe", "Turkish"},
			{"Ýleri", "Next"},
			{"Bulduðunuz cevabý buradan yazabilirsiniz.", "You can write the answer you find here."},
			{"Cevabýnýzý buradan onaylayabilirsiniz.", "You can confirm your answer here."},
			{"Buradan ipucu alabilirsiniz.", "You can get tips from here."},
			{"Sýradaki kelimeye geçebilirsiniz.", "You can move on to the next word."},
			{"Ýngilizce", "English"},
			{"ÝPUCU", "TIPS"},
			{"Kelime Bulmaca", "Word Puzzle"},
			{"Oyun 1 adet ipucu ve boþ kutular ile açýlýr. Ýpucu ile kelimeyi tahmin edebilir veya daha fazla ipucu almak için butona týklayabilirsin. Ne kadar az ipucu ile bilirsen o kadar yüksek puan alýrsýn. Her baþarýsýz tahminin ile birlikte yeni ipucu açýlmaya devam eder. Son ipucuyu açtýktan sonra yanlýþ cevap verirsen diðer soruya geçersin. Bölüm sonunda 50 veya daha yüksek puan alýrsan bir sonraki bölüm açýlýr.", "The game starts with one clue and empty boxes. You can either guess the word using the clue or click the button to get more clues. The fewer clues you use, the higher your score will be. With each incorrect guess, a new clue will be revealed. If you give a wrong answer after revealing the last clue, you will move on to the next question. If you score 50 or more points at the end of the level, the next level will be unlocked."},
			{"Enerjin bitti. Süreyi takip edip enerjinin dolmasýný bekleyebilir ya da reklam izleyerek enerjini doldurabilirsin!", "Your energy is depleted. You can either wait for the timer to replenish your energy or watch an ad to refill it!"},
			{"Toplam Puan", "Total Score"},
            {"Kelime Ýpucu", "Word Tips"},
			{"Yalnýzca kelime halinde ipuçlarý", "Tips in words only"},
			{"Ýpuçlarý cümle halinde olduðu için bu, daha basit bir oyun modu!", "Since the clues are in sentence form, this is a simpler game mode!"},
			{"Enerjin Bitti!", "Energy is depleted!"},
			{"REKLAM ÝLE ENERJÝ DOLDUR", "REFILL ENERGY WITH AN AD" },
			{"REKLAM ÝLE 10 ÝPUCU AL", "REFILL 10 TIPS WITH AN AD" },
			{"Enerjinin yenilenmesi için bekleyebilir veya reklam izleyerek enerjini doldurabilirsin!", "You can wait for your energy to replenish or watch an ad to refill it!"},
			{"Cümle Ýpucu", "Sentence Tips"},
			{"Menüye Dön", "Back To Menu"},
			{"Genel Kültür", "General Knowlegde"},
			{"Klasik genel kültür sorularýyla uzun bir macera", "A long adventure with classic general knowledge questions"},
			{"1. Ýpucu", "Tip1"},
			{"2. Ýpucu", "Tip2"},
			{"3. Ýpucu", "Tip3"},
			{"4. Ýpucu", "Tip4"},
			{"Matematik", "Maths"},
			{"Hukuk", "Law"},
			{"Ülkeler", "Countries"},
			{"Tarih", "History"},
			{"5. Ýpucu", "Tip5"},
			{"Bonus Soru", "Bonus Game"},
			{"Level:", "Chapter:"},
			{"Bonus soru ile bölümü tamamlayabilirsin", "Complete the section with the bonus game"},
			{"Sonraki Kelime", "Next Word"},
			{"Ýpucu Ver", "Give Tips"},
			{"Onayla", "Confirm"},
			{"Yakýnda..", "Coming Soon.."},
			{"???", "???"},
			{"Bölümü Tamamlayamadýn", "Chapter Failed"},
			{"Bölümü Tamamladýn", "Chapter Complete"},
			{"Bölümlere Dön", "Back To Chapter"},
			{"Harf Ver", "Give Letter"},
			{"Bölümü Bitir", "Back To Chapters"},
			{"Ýpuçlarý Bitti!", "Your Tips Over!"}









		};

	}

}
