using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;
using System;

public class HighscoreTable : MonoBehaviour {

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;

    private void Awake() 
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null) 
        {
            AddHighscoreEntry(89, "Creator", 0, 61);


            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++) 
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++) 
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
                else if (highscores.highscoreEntryList[j].score == highscores.highscoreEntryList[i].score)
                {
                    if(highscores.highscoreEntryList[j].deathCount < highscores.highscoreEntryList[i].deathCount)
                    {
                        HighscoreEntry tmp = highscores.highscoreEntryList[i];
                        highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                        highscores.highscoreEntryList[j] = tmp;
                    }
                    else if(highscores.highscoreEntryList[j].deathCount == highscores.highscoreEntryList[i].deathCount)
                    {
                        if(highscores.highscoreEntryList[j].time < highscores.highscoreEntryList[i].time)
                        {
                            HighscoreEntry tmp = highscores.highscoreEntryList[i];
                            highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                            highscores.highscoreEntryList[j] = tmp;
                        }
                    }
                }
            }
        }

        if (highscores.highscoreEntryList.Count > 12)
        {
            for (int h = highscores.highscoreEntryList.Count - 1; h >= 12; h--)
            {
                highscores.highscoreEntryList.RemoveAt(h);
            }
        }

        highscoreEntryTransformList = new List<Transform>();

        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
        
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) 
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1: rankString = ""; break;
            case 2: rankString = "1ST"; break;
            case 3: rankString = "2ND"; break;
            case 4: rankString = "3RD"; break;
            default: rankString = rank + "TH"; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = string.Format("{0}/89", score);

        int death = highscoreEntry.deathCount;
        entryTransform.Find("deathText").GetComponent<Text>().text = death.ToString();

        TimeSpan TS = new TimeSpan(0, 0, (int)highscoreEntry.time);
        entryTransform.Find("timeText").GetComponent<Text>().text = string.Format("{0:d2}:{1:d2}", TS.Minutes, TS.Seconds);

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = string.Format("{0}", name);

        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        
        if (rank == 1) {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("deathText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("timeText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        switch (rank)
        {
            case 1:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("FF0000");
                entryTransform.Find("trophy").GetComponent<RectTransform>().anchoredPosition = new Vector2(-210f, 1.6f);
                break;
            case 2:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("FFD200");
                break;
            case 3:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("C6C6C6");
                break;
            case 4:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("B76F56");
                break;
            default:
                entryTransform.Find("trophy").gameObject.SetActive(false);
                break;
        }

        transformList.Add(entryTransform);
    }

    public void AddHighscoreEntry(int score, string name, int deathCount, float time) 
    {
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, deathCount = deathCount, time = time, name = name };
        
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null) 
        {
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() };
        }

        highscores.highscoreEntryList.Add(highscoreEntry);

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores 
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable] 
    private class HighscoreEntry 
    {
        public int score;
        public string name;
        public int deathCount;
        public float time;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main_menu");
    }

}
