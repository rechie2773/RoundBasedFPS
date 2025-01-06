using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSettings : MonoBehaviour
{
    public void potato()
    {
        QualitySettings.SetQualityLevel(0);
    }
    public void low()
    {
        QualitySettings.SetQualityLevel(1);
    }
    public void med()
    {
        QualitySettings.SetQualityLevel(2);
    }
    public void highest()
    {
        QualitySettings.SetQualityLevel(3);
    }
}
