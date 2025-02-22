﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateController : MonoBehaviour {

    public Canvas UI;
    public Canvas PauseMenu;
    public float PlayerBrightnessBeginValue;
    public float PlayerBrightnessDecreaseRatePerSec;

    public GameObject InstructionsPanel;
    public GameObject InstructionsButton;
    public TextMeshProUGUI InstructionsText;

    private float PlayerBrightness;
    private UIManager UIManager;
    private PlayerController playerController;
    private GameObject ProgressSaver;

    private bool isGamePaused;

    void Awake()
    {
        UIManager = UI.GetComponent<UIManager>();
        GameObject player = GameObject.Find("Player");
        ProgressSaver = GameObject.Find("ProgressSaver");
        if (player == null)
            Debug.Log("GameStateController: Can not find Player in scene.\n");
        playerController = player.GetComponent<PlayerController>();
        isGamePaused = false;
    }

    // Use this for initialization
    void Start () {
        PlayerBrightness = PlayerBrightnessBeginValue;
        StartCoroutine(BeginCountDownThenStart());
    }
	
	// Update is called once per frame
	void Update () {
        if(!isGamePaused)
        {
            PlayerBrightness -= PlayerBrightnessDecreaseRatePerSec * Time.deltaTime;
            PlayerBrightness = Mathf.Clamp(PlayerBrightness, 0.0f, 100.0f);
            if(PlayerBrightness <= 0.0f)
            {
                OnPlayerLose();
            }
        }
        UIManager.SetPlayerBrightnessFiller(PlayerBrightness / 100.0f);
    }

    public void OnPlayerLose()
    {
        UIManager.SetFinalTimeText();
        UIManager.SetFinalScoreText(false);
        UIManager.ToggleFinalScorePanel(false);
        OnGamePause(false);
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void OnPlayerWin()
    {
        UIManager.SetFinalTimeText();
        UIManager.SetFinalScoreText(true);
        UIManager.ToggleFinalScorePanel(true);
        OnGamePause(false);
        if(ProgressSaver!=null)
        {
            Debug.Log("Progress Saved");
            ProgressSaver.GetComponent<SaveProgress>().SaveLevelProgress();
        }

        //UnityEditor.EditorApplication.isPlaying = false;
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            isGamePaused = true;
            OnGamePause(true);
        }
        else
        {
            isGamePaused = false;
            OnGameResume();
        }
    }

    public void OnGamePause(bool showPauseMenu)
    {
        isGamePaused = true;
        UIManager.GetComponent<Timer>().isPaused = true;
        if(showPauseMenu)
        {
            UI.enabled = false;
            PauseMenu.enabled = true;
        }
        playerController.OnPause();
    }

    public void OnGameResume()
    {
        isGamePaused = false;
        UIManager.GetComponent<Timer>().isPaused = false;
        if(UI.enabled == false)
        {
            UI.enabled = true;
            PauseMenu.enabled = false;
        }
        playerController.OnResume();

    }

    public void SetPlayerBrightnessDiff(float val)
    {
        PlayerBrightness += val;
        PlayerBrightness = Mathf.Clamp(PlayerBrightness, 0.0f, 100.0f);
    }

    private IEnumerator BeginCountDownThenStart()
    {
        playerController.SetSpeed(2.0f);
        OnGamePause(false);
        for (int sec = 3; sec > 0; sec--)
        {
            UIManager.GameStateAnnounceText.text = sec.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        UIManager.GameStateAnnounceText.text = "";
        UIManager.GameStateAnnounceText.enabled = false;
        OnGameResume();
    }

    public void ResumeGameFromTutorial()
    {
        if(InstructionsPanel != null && InstructionsText != null && InstructionsButton !=null)
        {
            InstructionsText.text = "";

            Time.timeScale = 1;

            InstructionsButton.gameObject.SetActive(false);
            InstructionsPanel.SetActive(false);
        }
    }
}
