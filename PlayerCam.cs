using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public float senX;
    public float senY;

    public Transform Orientation;

    float xRotate;
    float yRotate;

    private Vector3 originalPosition;

    private void Start()
    {
        // camera pos
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unlock cursor in specific scenes
        if (SceneManager.GetActiveScene().name == "MissionSuccess" ||
            SceneManager.GetActiveScene().name == "GameOverFog")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

        yRotate += mouseX;
        xRotate -= mouseY;

        xRotate = Mathf.Clamp(xRotate, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotate, 0);
    }

    public void DoFOV(float fov)
    {
        GetComponent<Camera>().DOFieldOfView(fov, 0.25f);
    }

    public void SetSensitivityX(float newSenX)
    {
        senX = newSenX;
    }

    public void SetSensitivityY(float newSenY)
    {
        senY = newSenY;
    }

    // Camera Shake Method
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition; // Reset to original position
    }
}
