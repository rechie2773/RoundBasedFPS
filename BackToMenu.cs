using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    //button trigger
    public void BackToMenuClick() 
    {
        //back to menu
        SceneManager.LoadScene("MainMenu");
    }
}
