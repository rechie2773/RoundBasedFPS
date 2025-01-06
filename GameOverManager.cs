using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip gameOverClip;

    [Header("Player Settings")]
    public PlayerHealth playerHealth;
    private bool hasPlayedGameOverSound = false;

    [Header("UI Settings")]
    public GameObject gameplayUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI roundsSurvivedText;
    public float fadeDuration = 2f;
    public float delayBeforeFade = 2f;

    [Header("Camera Settings")]
    public Camera gameOverCamera1;
    public Camera gameOverCamera2;
    public float cameraSwitchDelay = 12f;
    private RoundManager roundManager;
    private List<Graphic> uiElementsToFade;
    [Header("Scene Settings")]
    public float gameOverSceneDelay = 24f; 
    public string gameOverSceneName = "MainMenu";

    private List<AudioSource> allAudioSources = new List<AudioSource>();


    public static bool isGameOver = false;
    private void Start()
    {
        roundManager = FindObjectOfType<RoundManager>();


        uiElementsToFade = new List<Graphic>();
        if (gameOverUI != null)
        {
            uiElementsToFade.AddRange(gameOverUI.GetComponentsInChildren<Graphic>());
            SetUIAlpha(0f);
            gameOverUI.SetActive(false); //hide UI
        }
    }
    public void HandleGameOver()
    {
        isGameOver = true; 
        LockCursor(false);//unlock mouse cursor
        StopAllOtherAudio();
        PlayGameOverAudio();

        if (gameOverCamera1 != null)
        {
            gameOverCamera1.gameObject.SetActive(true);
        }
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        UpdateRoundsSurvivedText();
        Invoke(nameof(SwitchToGameOverCamera2), cameraSwitchDelay);
        StartCoroutine(FadeInUI());
        Invoke(nameof(LoadGameOverScene), gameOverSceneDelay);
    }

    private void PlayGameOverAudio()
    {
        if (!hasPlayedGameOverSound && audioSource != null && gameOverClip != null)
        {
            audioSource.clip = gameOverClip;
            audioSource.loop = false;
            audioSource.Play();
            hasPlayedGameOverSound = true;
        }
    }
    private void UpdateRoundsSurvivedText()
    {
        if (roundsSurvivedText != null && roundManager != null)
        {
            roundsSurvivedText.text = $"You survived {roundManager.currentRound} rounds";
        }
        else if (roundsSurvivedText != null)
        {
            roundsSurvivedText.text = "You survived 0 rounds";
        }
    }
    private void SwitchToGameOverCamera2()
    {
        if (gameOverCamera1 != null)
        {
            gameOverCamera1.gameObject.SetActive(false);
        }
        if (gameOverCamera2 != null)
        {
            gameOverCamera2.gameObject.SetActive(true);
        }
    }
    private IEnumerator FadeInUI()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            SetUIAlpha(alpha);

            yield return null;
        }

        SetUIAlpha(1f);
    }

        private void SetUIAlpha(float alpha)
        {
            foreach (Graphic graphic in uiElementsToFade)
            {
                if (graphic != null)
                {
                    Color color = graphic.color;
                    color.a = alpha;
                    graphic.color = color;
                }
            }
        }
    private void LockCursor(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
    private void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
    //stop all current audio in entire scene
    private void StopAllOtherAudio()
    {
        allAudioSources.AddRange(FindObjectsOfType<AudioSource>());

        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource) // Exclude the Game Over audio source
            {
                source.Stop();
            }
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGameOver = false;
    }
}

