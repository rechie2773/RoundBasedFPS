using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    public static PointSystem Instance; 
    private int points;
    private int totalPointsEarned;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int amount)
    {
        totalPointsEarned += amount;
        points += amount;
        Debug.Log($"Points added: {amount}. Total Points: {points}");
        // Update UI or other elements if necessary
    }

    public int GetPoints()
    {
        return points;
    }
    public void RemovePoints(int amount)
    {
        if (points >= amount) //check if there's enough points to minus
        {
            points -= amount;
            Debug.Log($"Points removed: {amount}. Remaining Points: {points}");
        }
        else
        {
            Debug.LogWarning("Not enough points to remove!");
        }
    }
    public int GetTotalPointsEarned()
    {
        return totalPointsEarned;
    }
}
