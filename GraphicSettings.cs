using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSettings : MonoBehaviour
{
	//lowest
=======
	//preset low
	//trien khai player
    public void potato()
    {
        QualitySettings.SetQualityLevel(0);
    }
	//low
    public void low()
    {
        QualitySettings.SetQualityLevel(1);
    }
	//medium
    public void med()
    {
        QualitySettings.SetQualityLevel(2);
    }
    public void highest()
    {
        QualitySettings.SetQualityLevel(3);
    }
}
