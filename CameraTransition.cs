using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public Transform targetPosition; //target position
    public float transitionDuration = 2f; 
    public AnimationCurve transitionCurve; 

    [Header("Auto Start")]
    public bool startOnEnable = true; // start if active

    private Vector3 initialPosition; // start position
    private Quaternion initialRotation; // cam's rotation
    private float elapsedTime; // time passed
    private bool isTransitioning; // camera during transition

    private void OnEnable()
    {
        if (startOnEnable)
        {
            StartTransition();
        }
    }

    private void StartTransition()
    {
        if (targetPosition == null)
        {
            Debug.LogWarning("Target position is not assigned!");
            return;
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        elapsedTime = 0f;
        isTransitioning = true;
    }

    private void Update()
    {
        if (isTransitioning)
        {
            HandleTransition();
        }
    }

    private void HandleTransition()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / transitionDuration);

        float curveValue = transitionCurve != null ? transitionCurve.Evaluate(t) : t;

        transform.position = Vector3.Lerp(initialPosition, targetPosition.position, curveValue);
        transform.rotation = Quaternion.Slerp(initialRotation, targetPosition.rotation, curveValue);

        if (t >= 1f)
        {
            isTransitioning = false;
        }
    }

    public void TriggerTransition()
    {
        StartTransition();
    }

    public void returnToOriginalPosition()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
