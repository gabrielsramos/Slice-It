using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private KnifeBehaviour _knife;
    [SerializeField] private FinishWall _finishWall;
    [SerializeField] private PowerUpBalloon _powerUpBalloon;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _currentScore;
    private bool _won;

    private void Start()
    {
        _won = false;
        _currentScore = 0;
        IncreaseScore(_currentScore);
    }

    private void OnEnable()
    {
        _knife.OnHitBlock += IncreaseScore;
        _knife.OnKnifeFall += GameOver;
        _finishWall.OnKnifeBladeCollided += GameWon;
        _powerUpBalloon.OnKnifeBladeCollided += ActivatePowerup;
    }

    private void OnDisable()
    {
        _knife.OnHitBlock -= IncreaseScore;
        _knife.OnKnifeFall -= GameOver;
        _finishWall.OnKnifeBladeCollided -= GameWon;
        _powerUpBalloon.OnKnifeBladeCollided -= ActivatePowerup;
    }

    private void ActivatePowerup()
    {
        _knife.ActivatePowerUp();
    }

    private void GameWon()
    {
        SaveScore();
        _won = true;
        LoadGameWonScreen();
    }

    private void GameOver()
    {
        SaveScore();
        LoadGameOverScreen();
    }

    private void LoadGameOverScreen()
    {
        SceneManager.LoadSceneAsync(Constants.GAME_OVER_SCENE_NAME, LoadSceneMode.Single);
    }

    private void LoadGameWonScreen()
    {
        SceneManager.LoadSceneAsync(Constants.GAME_WON_SCENE_NAME, LoadSceneMode.Single);
    }

    private void IncreaseScore(int points)
    {
        _currentScore += points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        _scoreText.text = $"SCORE: {_currentScore}";
    }

    private void SaveScore()
    {
        int wonValue = _won ? 1 : 0;
        PlayerPrefs.SetInt(Constants.LAST_SCORE_KEY, _currentScore);
        PlayerPrefs.SetInt(Constants.WON_KEY, wonValue);
        PlayerPrefs.Save();
    }
}
