using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private AudioSource overWaterSound;
    [SerializeField] private AudioSource underWaterSound;
    [SerializeField] private AirBar airBar;

    [SerializeField] private Spawner[] spawners;
    [SerializeField] private Countdown countdown;

    public event Action OnGameOver;
    private String highscorePrefsKey = "Highscore";

    public TMP_Text scoreText;
    private int score;

    //public Image airBar;
    public float airMax = 100;
    public float airCurrent;
    public float airSpeed = 1f;

    public event Action<int> OnCountTime;
    public int time;
    public TMP_Text timeText;
    private Coroutine countdownCoroutine;

    private void Start()
    {
        airCurrent = airMax;
        //countdownCoroutine = StartCoroutine(countTime());
        //Invoke(nameof(GameOver), 3.0f);
    }

    public void StartGame()
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.gameObject.SetActive(true);
        }
        countdown.gameObject.SetActive(false);
        countdownCoroutine = StartCoroutine(countTime());
        player.isDead = false;
    }

    private void Update()
    {
        if (!player.isUnderWater)
        {
            overWaterSound.UnPause();
            underWaterSound.Pause();
        }
        else
        {
            overWaterSound.Pause();
            underWaterSound.UnPause();
        }
    }

    private IEnumerator countTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            time++;
            int minutes = time / 60; 
            int seconds = time % 60;
            timeText.text = minutes + ":" + String.Format("{0:D2}", seconds);
            OnCountTime?.Invoke(time);
        }

    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void DecreaseAir()
    {
        airCurrent--;
        airBar.UpdateAirBar(airCurrent, airMax);
        if (airCurrent <= 0)
        {
            player.PlayerDie();
            StopCoroutine(countdownCoroutine);
            player.enabled = false;
            Invoke(nameof(GameOver), 3.0f);
        }
    }

    public void IncreaseAir(float newAir)
    {
        airCurrent = Mathf.Clamp(airCurrent + newAir*airSpeed, 0, airMax);
        airBar.UpdateAirBar(airCurrent, airMax);
    }

    public void ReloadAir(float newAir)
    {
        airCurrent = Mathf.Clamp(airCurrent + newAir, 0, airMax);
        airBar.UpdateAirBar(airCurrent, airMax);
    }

    public void PlayerDead()
    {
        StopCoroutine(countdownCoroutine);
        player.enabled = false;
        Invoke(nameof(GameOver), 3.0f);
    }

    public void GameOver()
    {
        player.gameObject.SetActive(false);
        OnGameOver?.Invoke();
    }

    public int GetScore()
    {
        return score;
    }

    public int GetHighscore()
    {
        int highscore = PlayerPrefs.GetInt(highscorePrefsKey, 0);
        return highscore;
    }

    public void SetHighscore(int highscore)
    {
        PlayerPrefs.SetInt(highscorePrefsKey, highscore);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
