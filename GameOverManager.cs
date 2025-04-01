using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý trạng thái kết thúc trò chơi, bao gồm UI, âm thanh, chuyển đổi camera và tải lại cảnh.
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [Header("Cài đặt Âm thanh")]
    public AudioSource audioSource;
    public AudioClip gameOverClip;

    [Header("Cài đặt Người chơi")]
    public PlayerHealth playerHealth;
    private bool hasPlayedGameOverSound = false;

    [Header("Cài đặt UI")]
    public GameObject gameplayUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI roundsSurvivedText;
    public float fadeDuration = 2f;
    public float delayBeforeFade = 2f;

    [Header("Cài đặt Camera")]
    public Camera gameOverCamera1;
    public Camera gameOverCamera2;
    public float cameraSwitchDelay = 12f;
    private RoundManager roundManager;
    private List<Graphic> uiElementsToFade;

    [Header("Cài đặt Cảnh")]
    public float gameOverSceneDelay = 24f; 
    public string gameOverSceneName = "MainMenu";

    private List<AudioSource> allAudioSources = new List<AudioSource>();

    public static bool isGameOver = false;

    /// <summary>
    /// Khởi tạo các tham chiếu cần thiết và chuẩn bị các yếu tố UI cho trạng thái kết thúc trò chơi.
    /// </summary>
    private void Start()
    {
        roundManager = FindObjectOfType<RoundManager>();

        uiElementsToFade = new List<Graphic>();
        if (gameOverUI != null)
        {
            uiElementsToFade.AddRange(gameOverUI.GetComponentsInChildren<Graphic>());
            SetUIAlpha(0f);
            gameOverUI.SetActive(false); // Ẩn UI ban đầu
        }
    }

    /// <summary>
    /// Xử lý tất cả các hành động cần thiết khi trò chơi kết thúc, chẳng hạn như dừng âm thanh, chuyển UI và chuyển đổi camera.
    /// </summary>
    public void HandleGameOver()
    {
        isGameOver = true; 
        LockCursor(false); // Mở khóa con trỏ chuột
        StopAllOtherAudio(); // Dừng tất cả âm thanh khác
        PlayGameOverAudio(); // Phát âm thanh kết thúc trò chơi

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
        UpdateRoundsSurvivedText(); // Cập nhật văn bản số vòng đã sống sót
        Invoke(nameof(SwitchToGameOverCamera2), cameraSwitchDelay); // Chuyển sang camera thứ hai
        StartCoroutine(FadeInUI()); // Bắt đầu làm mờ UI
        Invoke(nameof(LoadGameOverScene), gameOverSceneDelay); // Tải lại cảnh kết thúc trò chơi sau một khoảng thời gian
    }

    /// <summary>
    /// Phát âm thanh kết thúc trò chơi một lần. Nó ngăn không cho âm thanh phát nhiều lần.
    /// </summary>
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

    /// <summary>
    /// Cập nhật văn bản UI cho biết người chơi đã sống sót bao nhiêu vòng.
    /// </summary>
    private void UpdateRoundsSurvivedText()
    {
        if (roundsSurvivedText != null && roundManager != null)
        {
            roundsSurvivedText.text = $"Bạn đã sống sót {roundManager.currentRound} vòng";
        }
        else if (roundsSurvivedText != null)
        {
            roundsSurvivedText.text = "Bạn đã sống sót 0 vòng"; // Mặc định nếu không có roundManager
        }
    }

    /// <summary>
    /// Chuyển camera từ camera kết thúc trò chơi 1 sang camera kết thúc trò chơi 2.
    /// </summary>
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

    /// <summary>
    /// Làm mờ các yếu tố UI theo một khoảng thời gian nhất định sau khi có một độ trễ.
    /// </summary>
    /// <returns>Trả về một IEnumerator cho coroutine.</returns>
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

    /// <summary>
    /// Thiết lập giá trị alpha cho tất cả các yếu tố UI để tạo hiệu ứng làm mờ.
    /// </summary>
    /// <param name="alpha">Giá trị alpha mục tiêu để thiết lập.</param>
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

    /// <summary>
    /// Khóa hoặc mở khóa con trỏ chuột dựa trên tham số được đưa vào.
    /// </summary>
    /// <param name="lockCursor">Nếu là true, sẽ khóa con trỏ. Nếu là false, sẽ mở khóa con trỏ.</param>
    private void LockCursor(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    /// <summary>
    /// Tải lại cảnh kết thúc trò chơi sau một khoảng thời gian.
    /// </summary>
    private void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    /// <summary>
    /// Dừng tất cả các nguồn âm thanh trong cảnh, ngoại trừ nguồn âm thanh kết thúc trò chơi.
    /// </summary>
    private void StopAllOtherAudio()
    {
        allAudioSources.AddRange(FindObjectsOfType<AudioSource>());

        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource) // Loại trừ nguồn âm thanh kết thúc trò chơi
            {
                source.Stop();
            }
        }
    }

    /// <summary>
    /// Đăng ký sự kiện khi cảnh được tải xong để thiết lập lại trạng thái kết thúc trò chơi.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Hủy đăng ký sự kiện khi đối tượng bị vô hiệu hóa.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Thiết lập lại trạng thái kết thúc trò chơi khi một cảnh mới được tải.
    /// </summary>
    /// <param name="scene">Cảnh vừa được tải.</param>
    /// <param name="mode">Chế độ tải cảnh.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGameOver = false;
    }
}
