using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public static int HealphCount;
    private Image HealphCounter;

    private void Start()
    {
        HealphCounter = GetComponent<Image>();
        HealphCount = 100;
    }

    private void Update()
    {
        HealphCounter.fillAmount = (float)HealphCount / (float)100;
    }
}