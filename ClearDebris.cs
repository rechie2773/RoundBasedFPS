using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClearDebris : MonoBehaviour
{
    [Header("Debris Settings")]
    public GameObject debris; 
    public Transform targetPos; 
    public float moveSpeed = 2f; 
    public int clearCost = 100; 

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clearSuccessClip;
    public AudioClip notEnoughPointsClip;

    [Header("UI Settings")]
    public TextMeshProUGUI debrisText; 

    [Header("Trigger Zone")]
    public GameObject triggerZone;
    private bool isDebrisCleared = false;

    private bool playerInRange = false;

    private void Start()
    {
        if (debrisText != null)
        {
            debrisText.gameObject.SetActive(false); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInRange = true;

            if (!isDebrisCleared && debrisText != null)
            {
                debrisText.text = $"Press [F] to Clear Debris - {clearCost} Points";
                debrisText.gameObject.SetActive(true);
            }

            Debug.Log("Player entered debris zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInRange = false;

            if (debrisText != null)
            {
                debrisText.gameObject.SetActive(false); 
            }

            Debug.Log("Player exited debris zone.");
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F)) 
        {
            TryClearDebris();
        }
    }

    private void TryClearDebris()
    {
        if (PointSystem.Instance.GetPoints() >= clearCost) 
        {
            //if clear debris success
            PointSystem.Instance.RemovePoints(clearCost);

            if (audioSource != null && clearSuccessClip != null)
            {
                audioSource.PlayOneShot(clearSuccessClip); 
            }
            if (debrisText != null)
            {
                debrisText.gameObject.SetActive(false);
            }
            isDebrisCleared = true;
            StartCoroutine(ClearDebrisRoutine());
        }
        else
        {
            if (audioSource != null && notEnoughPointsClip != null)
            {
                audioSource.PlayOneShot(notEnoughPointsClip); 
            }

            Debug.Log("Not enough points to clear debris.");
        }
    }

    private IEnumerator ClearDebrisRoutine()
    {
        if (debris != null && targetPos != null)
        {
            while (Vector3.Distance(debris.transform.position, targetPos.position) > 0.1f)
            {
                debris.transform.position = Vector3.MoveTowards(
                    debris.transform.position,
                    targetPos.position,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            Destroy(debris);

            if (triggerZone != null)
            {
                Destroy(triggerZone);
            }
        
        }
    }
}
