using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image blackoutPanel;

    private void Start()
    {
        StartCoroutine(Fade(true));
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
