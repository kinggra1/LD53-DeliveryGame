using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private enum GameState { PLAYING, PAUSED, GAMEOVER }
    private GameState gameState = GameState.PLAYING;

    public GameObject gameplayUI;
    public GameObject gameOverUI;
    public TMP_Text gameOverScore;

    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToScore(int amount) {
        score += amount;
        UpdateGameplayUI();
    }

    public bool IsPlaying() {
        return gameState == GameState.PLAYING;
    }

    public bool IsPaused() {
        return gameState == GameState.PAUSED;
    }

    public bool IsGameOver() {
        return gameState == GameState.GAMEOVER;

    }

    public void GameOver() {
        gameState = GameState.GAMEOVER;
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(true);
        gameOverScore.text = "Final Score: " + score;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateGameplayUI() {
        
    }
}
