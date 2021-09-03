using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image healthbar;
    public Image bossHealthbar;
    public Image jump;
    public Image dash;
    public Image torchberry;
    public Image damageScreen;
    public Text alertText;
    public GameObject pauseMenu;
    public GameObject bossMenu;
    private float startingWidth;
    private float startingBossWidth;
    private EnemyBehaviour boss;

    private void Start()
    {
        startingWidth = healthbar.rectTransform.sizeDelta.x;
        startingBossWidth = bossHealthbar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (damageScreen.color.a > 0)
        {
            damageScreen.color = new Color(damageScreen.color.r, 0, 0, damageScreen.color.a - (0.5f * Time.deltaTime));
        }
        if (boss != null)
        {
            bossHealthbar.rectTransform.sizeDelta = new Vector2(startingBossWidth * ((float)boss.currentHP / boss.maxHP), bossHealthbar.rectTransform.sizeDelta.y);
        }
        else if (bossMenu.activeSelf)
        {
            bossMenu.SetActive(false);

        }
    }

    public void UpdateHealthbar(int currentHP, int maxHP)
    {
        healthbar.rectTransform.sizeDelta = new Vector2(startingWidth * ((float)currentHP / maxHP), healthbar.rectTransform.sizeDelta.y);
    }

    public void SetJump(bool can)
    {
        if (can)
        {
            jump.color = new Color32(100, 238, 121, 90);
        }
        else
        {
            jump.color = new Color32(113, 113, 113, 90);
        }
    }

    public void SetDash(bool can)
    {
        if (can)
        {
            dash.color = new Color32(255, 77, 0, 90);
        }
        else
        {
            dash.color = new Color32(113, 113, 113, 90);
        }
    }

    public void TakeDamage(int d)
    {
        damageScreen.color = new Color(damageScreen.color.r, 0, 0, Mathf.Clamp(damageScreen.color.a + (d / 20f), 0, 0.5f));
    }

    public void SetTorchberry(bool can)
    {
        torchberry.gameObject.SetActive(can);
    }

    public void Alert(string text, int seconds)
    {
        StopAllCoroutines();
        StartCoroutine(AlertCoroutine(text, seconds));
    }

    public void ActivatePauseMenu(bool state, string text = "")
    {
        Alert(text, 0);
        pauseMenu.SetActive(state);
    }

    public void ShowBoss(EnemyBehaviour bossEntity)
    {
        boss = bossEntity;
        bossMenu.SetActive(true);
    }

    IEnumerator AlertCoroutine(string text, int seconds)
    {
        alertText.text = text;
        alertText.color = new Color32(180, 235, 235, 255);
        if (seconds != 0)
        {
            yield return new WaitForSecondsRealtime(seconds);
            for (int i = 0; i < 20; i++)
            {
                alertText.color = new Color32(180, 235, 235, (byte)(255 - Mathf.RoundToInt(255 * ((float)i + 1) / 20)));
                yield return null;
            }
        }
    }
}
