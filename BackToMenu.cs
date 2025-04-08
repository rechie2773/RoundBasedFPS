using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//back to menu button
public class BackToMenu : MonoBehaviour
{
    //button
    public void BackToMenuClick() 
    {
        //back to menu
        SceneManager.LoadScene("MainMenu");
    }
}
