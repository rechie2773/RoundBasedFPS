using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggManager : MonoBehaviour
{
    [Header("Easter Egg Settings")]
    public List<GameObject> easterEggObjects; // List of GameObjects to destroy
    public int rewardPoints = 10000; // Points to award when completed

    private void Start()
    {
        if (easterEggObjects.Count == 0)
        {
            Debug.LogWarning("No objects added to the Easter Egg Manager!");
        }
    }

    // Function to call when an object is destroyed
    public void OnObjectDestroyed(GameObject destroyedObject)
    {
        if (easterEggObjects.Contains(destroyedObject))
        {
            easterEggObjects.Remove(destroyedObject); // Remove the destroyed object from the list

            Debug.Log($"Object {destroyedObject.name} destroyed. Remaining objects: {easterEggObjects.Count}");

            // Check if the list is empty
            if (easterEggObjects.Count == 0)
            {
                RewardPlayer();
            }
        }
    }

    private void RewardPlayer()
    {
        Debug.Log("Easter Egg Completed! Awarding points...");

        PointSystem pointSystem = FindObjectOfType<PointSystem>();
        if (pointSystem != null)
        {
            pointSystem.AddPoints(rewardPoints);
            Debug.Log($"Player awarded {rewardPoints} points!");
        }
        else
        {
            Debug.LogError("PointSystem not found in the scene!");
        }
    }
}
