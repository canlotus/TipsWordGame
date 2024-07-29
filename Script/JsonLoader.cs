using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;

public static class JsonLoader
{
    private static readonly string[] jsonFiles = new string[]
    {
        "chapter1.json", "chapter2.json", "chapter3.json", "chapter4.json", "chapter5.json",
        "chapter6.json", "chapter7.json", "chapter8.json", "chapter9.json", "chapter10.json",
        "chapter11.json", "chapter12.json", "chapter13.json", "chapter14.json", "chapter15.json",
        "chapter16.json", "chapter17.json", "chapter18.json", "chapter19.json", "chapter20.json",
        "chapter21.json", "chapter22.json", "chapter23.json", "chapter24.json", "chapter25.json",
        "chapter26.json", "chapter27.json", "chapter28.json", "chapter29.json", "chapter30.json",
        "chapter31.json", "chapter32.json", "chapter33.json", "chapter34.json", "chapter35.json",
        "chapter36.json", "chapter37.json", "chapter38.json", "chapter39.json", "chapter40.json",
        "chapter41.json", "chapter42.json", "chapter43.json", "chapter44.json", "chapter45.json",
        "chapter46.json", "chapter47.json", "chapter48.json", "chapter49.json", "chapter50.json",
        "chapter51.json", "chapter52.json", "chapter53.json", "chapter54.json", "chapter55.json",
        "chapter56.json", "chapter57.json", "chapter58.json", "chapter59.json", "chapter60.json",
        "chapter61.json", "chapter62.json", "chapter63.json", "chapter64.json", "chapter65.json",
        "chapter66.json", "chapter67.json", "chapter68.json", "chapter69.json", "chapter70.json",
        "chapter71.json", "chapter72.json", "chapter73.json", "chapter74.json", "chapter75.json",
        "chapter76.json", "chapter77.json", "chapter78.json", "chapter79.json", "chapter80.json", "WordChapter1.json", "WordChapter2.json", "WordChapter3.json",
        "WordChapter4.json", "WordChapter5.json", "WordChapter6.json", "WordChapter7.json", "WordChapter8.json",
        "WordChapter9.json", "WordChapter10.json", "WordChapter11.json", "WordChapter12.json", "WordChapter13.json",
        "WordChapter14.json", "WordChapter15.json", "WordChapter16.json", "WordChapter17.json", "WordChapter18.json",
        "WordChapter19.json", "WordChapter20.json", "WordChapter21.json", "WordChapter22.json", "WordChapter23.json",
        "WordChapter24.json", "WordChapter25.json", "WordChapter26.json", "WordChapter27.json", "WordChapter28.json",
        "WordChapter29.json", "WordChapter30.json", "WordChapter31.json", "WordChapter32.json", "WordChapter33.json",
        "WordChapter34.json", "WordChapter35.json", "WordChapter36.json", "WordChapter37.json", "WordChapter38.json",
        "WordChapter39.json", "WordChapter40.json", "WordChapter41.json", "WordChapter42.json", "ChapterBonus.json",
        "WordChapterBonus.json"
    };

    public static async Task<string> LoadJsonFromStreamingAssets(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            www.SendWebRequest();

            while (!www.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load JSON file: {filePath}");
                return null;
            }

            return www.downloadHandler.text;
        }
        else
        {
            return File.ReadAllText(filePath);
        }
    }

    public static async Task<Dictionary<string, string>> LoadAllJsonFiles()
    {
        Dictionary<string, string> jsonContents = new Dictionary<string, string>();

        foreach (string fileName in jsonFiles)
        {
            string content = await LoadJsonFromStreamingAssets(fileName);
            if (!string.IsNullOrEmpty(content))
            {
                jsonContents[fileName] = content;
            }
        }

        return jsonContents;
    }
}
