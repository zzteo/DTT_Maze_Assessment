using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private Toggle _showFpsToggle;

    [SerializeField] private Toggle _vSync;

    void Update()
    {
        _fpsText.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }

    public void ShowFps()
    {
        if (_showFpsToggle.isOn)
        {
            _fpsText.gameObject.SetActive(true);
        }
        else
        {
            _fpsText.gameObject.SetActive(false);
        }
    }
    public void ToggleVsync()
    {
        if (!_vSync.isOn)       
            QualitySettings.vSyncCount = 0;       
        else
            QualitySettings.vSyncCount = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
