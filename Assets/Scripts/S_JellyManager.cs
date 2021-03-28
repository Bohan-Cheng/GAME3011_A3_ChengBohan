using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_Difficulty
{
    Easy,
    Medium,
    Hard
}

public class S_JellyManager : MonoBehaviour
{
    public E_Difficulty difficulty = E_Difficulty.Easy;
    public int MaxStones = 5;
    public int Score = 0;
    [SerializeField] Text ModeText;
    [SerializeField] Text ScoreText;

    void Start()
    {
        ScoreText.text = Score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EasyMode()
    {
        difficulty = E_Difficulty.Easy;
        MaxStones = 0;
        FindObjectOfType<S_JellyTable>().ResetTable();
        ModeText.text = "Mode: Easy";
    }

    public void MediumMode()
    {
        difficulty = E_Difficulty.Medium;
        MaxStones = 0;
        FindObjectOfType<S_JellyTable>().ResetTable();
        ModeText.text = "Mode: Medium";
    }

    public void HardMode()
    {
        difficulty = E_Difficulty.Hard;
        MaxStones = 10;
        FindObjectOfType<S_JellyTable>().ResetTable();
        ModeText.text = "Mode: Hard";
    }

    public void AddScore(int score)
    {
        Score += score;
        ScoreText.text = Score.ToString();
    }
}
