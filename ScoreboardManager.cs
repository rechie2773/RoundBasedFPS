using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardManager : MonoBehaviour
{
    [Header("Scoreboard UI")]
    public GameObject scoreboardUI; 
    public TextMeshProUGUI roundsSurvivedText; 
    public TextMeshProUGUI zombiesKilledText; 
    public TextMeshProUGUI totalPointsText; 
    private RoundManager roundManager;
    private int zombiesKilled = 0; 
    private PointSystem pointSystem;

    [Header("Other UI")]
    public List<GameObject> otherUIElements;

    private void Start()
    {
        scoreboardUI.SetActive(false); 

        roundManager = FindObjectOfType<RoundManager>();
        pointSystem = PointSystem.Instance;
    }

    private void Update()
    {
        if (GameOverManager.isGameOver) return;
        //check if pause is active
        if (PauseMenu.paused) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowScoreboard();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreboard();
        }
    }

    public void ZombieKilled()
    {
        zombiesKilled++; 
    }

    private void ShowScoreboard()
    {
        if (roundManager != null && pointSystem != null)
        {
            
            roundsSurvivedText.text = $"Rounds Survived: {roundManager.currentRound}";
            zombiesKilledText.text = $"Zombies Killed: {zombiesKilled}";
            int totalPointsEarned = PointSystem.Instance.GetTotalPointsEarned();
            totalPointsText.text = $"Total Points Earned: {totalPointsEarned}";
        }
        SetOtherUIActive(false);
        scoreboardUI.SetActive(true); 
    }

    private void HideScoreboard()
    {
        SetOtherUIActive(true);
        scoreboardUI.SetActive(false);
    }
    private void SetOtherUIActive(bool isActive)
    {
        foreach (GameObject uiElement in otherUIElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(isActive);
            }
        }
    }
}
