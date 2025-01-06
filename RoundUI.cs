using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class RoundUI : MonoBehaviour
{
    public TextMeshProUGUI roundText; 
    private RoundManager roundManager;

    [Header("Transition Settings")]
    public RectTransform roundTextTransform; 
    public Vector3 centerPosition = new Vector3(0,500, 0); 
    public float transitionDuration = 1f; 
    public float holdTime = 1f; 
    private Vector3 originalPosition; 
    private Vector3 originalScale;

    [Header("Round Announcer text")]
    public TextMeshProUGUI roundStartText; 
    public string startTextMessage = "ROUND"; 
    public float startTextFadeDuration = 0.5f;

    private void Start()
    {
        roundManager = FindObjectOfType<RoundManager>();
        if (roundTextTransform != null)
        {
            originalPosition = roundTextTransform.anchoredPosition;
            originalScale = roundTextTransform.localScale;
        }

        if (roundStartText != null)
        {
            roundStartText.alpha = 0; //hide text
        }
    }

    public void TriggerRoundTransition(int roundNumber)
    {
        if (roundTextTransform == null) return;

       //update current round
        roundText.text = $"{roundNumber}";

        ShowRoundStartText();

        roundTextTransform.DOAnchorPos(centerPosition, transitionDuration).SetEase(Ease.OutBack);
        roundTextTransform.DOScale(2.0f, transitionDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
           
            DOVirtual.DelayedCall(holdTime, () =>
            {
                
                roundTextTransform.DOAnchorPos(originalPosition, transitionDuration).SetEase(Ease.InBack);
                roundTextTransform.DOScale(originalScale, transitionDuration).SetEase(Ease.InBack);

                HideRoundStartText();
            });
        });
    }
    private void ShowRoundStartText()
    {
        if (roundStartText != null)
        {
            roundStartText.text = startTextMessage; 
            roundStartText.DOFade(1, startTextFadeDuration); 
        }
    }

    private void HideRoundStartText()
    {
        if (roundStartText != null)
        {
            roundStartText.DOFade(0, startTextFadeDuration); 
        }
    }
}
