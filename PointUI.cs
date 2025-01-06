using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointUI : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    private void Update()
    {
        if (PointSystem.Instance != null)
        {
            pointsText.text = $"{PointSystem.Instance.GetPoints()}";
        }
    }
}
