using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
    public AudioMixer am;
    Resolution[] rsl;
    public Dropdown resolution;
    private static string res;
    List<string> resolutions;
    private static bool fullscreenT;
    private static float soundValue;
    public Slider slider;
    public Dropdown quality;
    public Toggle fullscreen;

    private void Awake()
    {
        resolutions = new List<string>();
        rsl = Screen.resolutions;
        foreach (var i in rsl)
        {
            resolutions.Add(i.width + "x" + i.height);
        }
        resolution.ClearOptions();
        resolution.AddOptions(resolutions);

        slider.value = soundValue;
        resolution.value = resolutions.IndexOf(res);
        quality.value = QualitySettings.GetQualityLevel();
        fullscreen.isOn = fullscreenT;
    }

    public void Resolution(int r)
    {
        Screen.SetResolution(rsl[r].width, rsl[r].height, Screen.fullScreen);
        res = rsl[r].width + "x" + rsl[r].height;
    }

    public void FullScreenToggle(bool t)
    {
        fullscreenT = t;
        Screen.fullScreen = t;
    }

    public void AudioVolume(float sliderValue)
    {
        soundValue = sliderValue;
        am.SetFloat("masterVolume", sliderValue);
    }

    public void Quality(int q)
    {
        QualitySettings.SetQualityLevel(q);
    }
}
