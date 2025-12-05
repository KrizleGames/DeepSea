using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private Button restartButton;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnGameOver += HandleGameOver;
    }

    private void OnDestroy()
    {
        gameManager.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        gameOverScreen.SetActive(true);
        scoreText.text = $"Punkte: {gameManager.GetScore()}";
        highscoreText.text = $"Highscore: {gameManager.GetHighscore()}";
        if (gameManager.GetScore() > gameManager.GetHighscore())
        {
            gameManager.SetHighscore(gameManager.GetScore());
        }
    }
}
