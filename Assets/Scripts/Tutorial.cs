using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Text tutorialText;
    public bool torchberryDone, damageCheatDone, healthCheatDone, readyDone;
    public bool gameStarted, tutorialPassed;
    private bool walkingDone, jumpingDone, attackingDone, throwingDone, dashingDone;

    private void Start()
    {
        gameStarted = false;
        tutorialPassed = PlayerPrefs.GetInt("TutorialDone", 0) == 1;
        UpdateText();
    }

    public void UpdateText()
    {
        if (!tutorialPassed)
        {
            if (walkingDone && jumpingDone && dashingDone && attackingDone && throwingDone && torchberryDone && damageCheatDone && healthCheatDone && readyDone)
            {
                tutorialText.text = "";
                GetComponent<GameManager>().StartWave();
                gameStarted = true;
                PlayerPrefs.SetInt("TutorialDone", 1);
            }
            else
            {
                string text = "";
                text += walkingDone ? "<color=green>Walk using WASD</color>\n" : "Walk using WASD\n";
                text += jumpingDone ? "<color=green>Jump using Space</color>\n" : "Jump using Space\n";
                text += dashingDone ? "<color=green>Dash using RMB</color>\n" : "Dash using RMB\n";
                text += attackingDone ? "<color=green>Attack using LMB</color>\n" : "Attack using LMB\n";
                text += throwingDone ? "<color=green>Throw a dagger using Q</color>\n" : "Throw a dagger using Q\n";
                text += torchberryDone ? "<color=green>Press E near a torchberry to heal</color>\n" : "Press E near a torchberry to heal\n";
                text += damageCheatDone ? "<color=green>Press Ctrl+E to activate damage cheat</color>\n" : "Press Ctrl+E to activate damage cheat\n";
                text += healthCheatDone ? "<color=green>Press Ctrl+R to activate godmode</color>\n" : "Press Ctrl+R to activate godmode\n";
                text += readyDone ? "<color=green>Press F when you're ready</color>\n" : "Press F when you're ready\n";
                text += "\nPress Ctrl+F to suicide. But not now!\n";
                text += "You can destroy projectiles with your attacks\n";
                text += "You can dodge enemy attacks if you are far enough\n";
                tutorialText.text = text;
            }
        }
        else
        {
            if (readyDone)
            {
                tutorialText.text = "";
                GetComponent<GameManager>().StartWave();
                gameStarted = true;
            }
            else
            {
                tutorialText.text = "Press F when you're ready";
            }
        }
    }

    private void Update()
    {
        if (!gameStarted)
        {
            if (!tutorialPassed)
            {
                if (!walkingDone && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
                {
                    walkingDone = true;
                    UpdateText();
                }
                if (!jumpingDone && Input.GetKeyDown(KeyCode.Space))
                {
                    jumpingDone = true;
                    UpdateText();
                }
                if (!attackingDone && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    attackingDone = true;
                    UpdateText();
                }
                if (!dashingDone && Input.GetKeyDown(KeyCode.Mouse1))
                {
                    dashingDone = true;
                    UpdateText();
                }
                if (!throwingDone && Input.GetKeyDown(KeyCode.Q))
                {
                    throwingDone = true;
                    UpdateText();
                }
            }
            if (!readyDone && Input.GetKeyDown(KeyCode.F))
            {
                readyDone = true;
                UpdateText();
            }
        }
    }
}
