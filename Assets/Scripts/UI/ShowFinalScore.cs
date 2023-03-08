using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowFinalScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _score;

    private void Start()
    {
        int score = PlayerPrefs.GetInt(Constants.LAST_SCORE_KEY, 0);
        _score.text = $"YOUR SCORE: {score}";
    }
}
