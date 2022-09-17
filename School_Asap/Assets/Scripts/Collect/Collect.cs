using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    [SerializeField]
    public static bool CollectKey;
    [SerializeField]
    public static bool CollectKeyEnd;
    [SerializeField]
    public static int CollectCrystal;
    [SerializeField]
    public static bool End;
    [SerializeField]
    public static float time;
    [SerializeField]
    public static int deathCount;
    [SerializeField]
    private Text CrystalCounter;


    private void Awake()
    {
        CollectCrystal = 0;
        deathCount = 0;
        time = 0;
        CollectKey = false;
        CollectKeyEnd = false;
    }

    public void NumberOfDeath()
    {
        deathCount++;
    }

    private void Update()
    {
        CrystalCounter.text = string.Format("{0}", CollectCrystal);
        time = time + Time.deltaTime;
    }
}
