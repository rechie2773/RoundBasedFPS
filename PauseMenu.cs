using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    public static bool paused ;
    public GameObject PauseMenuCanvas;

    //audio stop (walking)
    private PlayerMovementAdvanced playerMovement;
    private List<AudioSource> allAudioSources = new List<AudioSource>();
    public Slider mouseSensitivitySlider; //adding slider for mouse
    public PlayerCam playerCam;
    [Header("UI Elements")]
    public List<GameObject> uiElementsToHide;

    void Start()
    {
        Time.timeScale = 1f;
        paused = false;
        playerMovement = FindObjectOfType<PlayerMovementAdvanced>();

        if (mouseSensitivitySlider != null && playerCam != null)
        {
            mouseSensitivitySlider.value = playerCam.senX; // suppose x and y are the same
            mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (GameOverManager.isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
        if (!paused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseAllAudio();
        if (playerMovement != null)
        {
            playerMovement.PauseAudio();
        }
        RoundManager roundManager = FindObjectOfType<RoundManager>();
        if (roundManager != null && roundManager.roundChangeAudio != null)
        {
            roundManager.roundChangeAudio.Pause();
        }
        PauseZombieAudio();

        GunAudioManager[] gunAudios = FindObjectsOfType<GunAudioManager>();
        foreach (GunAudioManager gunAudio in gunAudios)
        {
            gunAudio.PauseAudio();
        }
        SetUIElementsActive(false);
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ResumeAllAudio();
        if (playerMovement != null)
        {
            playerMovement.ResumeAudio();
        }
        RoundManager roundManager = FindObjectOfType<RoundManager>();
        if (roundManager != null && roundManager.roundChangeAudio != null)
        {
            roundManager.roundChangeAudio.UnPause();
        }
        ResumeZombieAudio();

        GunAudioManager[] gunAudios = FindObjectsOfType<GunAudioManager>();
        foreach (GunAudioManager gunAudio in gunAudios)
        {
            gunAudio.ResumeAudio();
        }
        SetUIElementsActive(true);
    }
    public void UpdateMouseSensitivity(float newSensitivity)
    {
        if (playerCam != null)
        {
            playerCam.SetSensitivityX(newSensitivity);
            playerCam.SetSensitivityY(newSensitivity);
        }
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void PauseZombieAudio()
    {
        EnemyAI[] zombies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI zombie in zombies)
        {
            if (zombie.audioSource != null)
            {
                zombie.audioSource.Pause();
            }
        }
    }
    private void ResumeZombieAudio()
    {
        EnemyAI[] zombies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI zombie in zombies)
        {
            if (zombie.audioSource != null)
            {
                zombie.audioSource.UnPause();
            }
        }
    }
    private void PauseAllAudio()
    {
        allAudioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        foreach (var audioSource in allAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    private void ResumeAllAudio()
    {
        foreach (var audioSource in allAudioSources)
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.UnPause();
            }
        }
    }
    private void SetUIElementsActive(bool isActive)
    {
        foreach (GameObject uiElement in uiElementsToHide)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(isActive);
            }
        }
    }
}


