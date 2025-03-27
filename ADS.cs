using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADS : MonoBehaviour
{
    [Header("Weapon / Camera")]
    [SerializeField] private Transform WeaponADSLayer;
    [SerializeField] private Camera _camera;

    [Header("Variables")]
    [SerializeField] private float smoothTime = 10f;
    [SerializeField] private float offsetX = 10f;
    [SerializeField] private float offsetY = 10f;
    [SerializeField] private float offsetZ = 10f;
    
    [SerializeField]public bool IsAiming { get; private set; } = false;
    [SerializeField] private float aimingFOV = 30f; 
    [SerializeField] private float defaultFOV = 60f;
    public FOVManager fovManager;
    [Header("Keys")]
    [SerializeField] private KeyCode ADSKey = KeyCode.Mouse1;

    private Vector3 originalWeaponPosition;
    
    // Khoi tao cac gia tri ban dau cho game
    private void Start()
    {
        _camera = GetComponentInParent<Camera>();
        originalWeaponPosition = WeaponADSLayer.localPosition;

        defaultFOV = _camera.fieldOfView;

        UpdateAiming(false);
        fovManager = FindObjectOfType<FOVManager>();
    }
    // cap nhat lai các frame cua cho troi
    private void Update()
    {
        myInput();
        HandleAiming();
    }

    //xu ly input ban cua nguoi choi
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1)) IsAiming = true;   
        if (Input.GetMouseButtonUp(1)) IsAiming = false;    
    }

    //xu ly camera cua nguoi choi
    private void HandleAiming()
    {
        if (IsAiming)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, aimingFOV, Time.deltaTime * smoothTime);

            // Calculate the target screen position at the center of the screen
            Vector3 targetScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            // Convert the screen position to a world position based on the weapon's distance from the camera
            float distanceFromCamera = Vector3.Distance(WeaponADSLayer.position, _camera.transform.position);
            Vector3 targetWorldPosition = _camera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, distanceFromCamera));

            // Convert the world position to a local position relative to the weapon
            Vector3 targetLocalPosition = WeaponADSLayer.parent.InverseTransformPoint(targetWorldPosition);

            // Apply the specified offsets
            targetLocalPosition += new Vector3(offsetX, offsetY, offsetZ);
            targetLocalPosition.z = offsetZ;

            // Lerp the weapon position to match the target position
            WeaponADSLayer.localPosition = Vector3.Lerp(WeaponADSLayer.localPosition, targetLocalPosition, Time.deltaTime * smoothTime);
        }
        else
        {
            if (fovManager != null)
            {
                fovManager.ApplyFOV(fovManager.fovSlider.value);
            }
            else
            {
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, defaultFOV, Time.deltaTime * smoothTime);
            }

            WeaponADSLayer.localPosition = Vector3.Lerp(WeaponADSLayer.localPosition, originalWeaponPosition, Time.deltaTime * smoothTime);
        }
    }

    //quan ly và xu ly cac input cua nguoi choi
    private void myInput()
    {
        if (Input.GetKeyDown(ADSKey))
        {
            UpdateAiming(true);
        }
        if (Input.GetKeyUp(ADSKey))
        {
            UpdateAiming(false);
        }
    }

    //cap nhat lai trang thai camera cua nguoi choi
    private void UpdateAiming(bool Aiming)
    {
        IsAiming = Aiming;       
    }
}
