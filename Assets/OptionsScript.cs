using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FpsText;
    [SerializeField] private Toggle ShowFpsToggle;

    [SerializeField] private Toggle Vsync;

    void Update()
    {
        FpsText.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }

    public void ShowFps()
    {
        if (ShowFpsToggle.isOn)
        {
            FpsText.gameObject.SetActive(true);
        }
        else
        {
            FpsText.gameObject.SetActive(false);
        }
    }
    public void ToggleVsync()
    {
        if (!Vsync.isOn)       
            QualitySettings.vSyncCount = 0;       
        else
            QualitySettings.vSyncCount = 1;
    }
}
