using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndDoor : MonoBehaviour
{
    public GameObject enterRecord;
    public HighscoreTable table;

    private void Update()
    {
        if(Collect.End)
        {
            Time.timeScale = 0;
            enterRecord.SetActive(true);
        }
    }

    public void SaveRecord()
    {
        Collect.End = false;
        Time.timeScale = 1;
        string name = enterRecord.transform.Find("InputField").transform.Find("Name").GetComponent<Text>().text;
        name = name.Trim();

        if (name == null || name == "")
            return;

        table.AddHighscoreEntry(Collect.CollectCrystal, name, Collect.deathCount, Collect.time);
    }
}