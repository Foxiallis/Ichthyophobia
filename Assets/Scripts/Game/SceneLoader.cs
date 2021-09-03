using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image blackoutPanel;
    public Slider slider;

    private void Start()
    {
        StartCoroutine(Fade(true));
        if (slider != null)
        {
            slider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
        }
    }

    public void Load(int id)
    {
        StartCoroutine(Fade(false, delegate { SceneManager.LoadScene(id); }));
    }

    public void LoadLevel(int waves)
    {
        GameManager.waveAmount = waves;
        Load(1);
    }

    public void UpdateMouseSensitivity() //this doesn't fit the scene loader but since I didn't do any other options for now it remains here
    {
        PlayerPrefs.SetFloat("MouseSensitivity", slider.value);
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator Fade(bool fadeIn, UnityAction callback = null)
    {
        for (int i = 0; i < 21; i++)
        {
            if (fadeIn)
            {
                blackoutPanel.color = new Color(0, 0, 0, 1 - (i / 20f));
            }
            else
            {
                blackoutPanel.color = new Color(0, 0, 0, i / 20f);
            }
            yield return null;
        }
        if (callback != null)
        {
            callback();
        }
    }
}
