using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private enum GameState { PLAYING, PAUSED, GAMEOVER }
    private GameState gameState = GameState.PLAYING;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
