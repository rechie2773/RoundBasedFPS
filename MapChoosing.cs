using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapChoosing : MonoBehaviour
{
    // This script is responsible for loading different maps in the game.
    public void FogClick()
    {
        // Clear all persistent objects if necessary
        ResetGameState();

        // Reload scene completely
        SceneManager.LoadScene("FPS_RoundBased");
    }

    private void ResetGameState()
    {
        GameObject[] objectsToDestroy = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj.tag != "Player")
            {
                Destroy(obj);
            }
        }
    }
}
