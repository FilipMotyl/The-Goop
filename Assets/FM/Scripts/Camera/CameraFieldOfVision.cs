using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFieldOfVision : MonoBehaviour
{
    private const float NORMAL_FOV = 60f;

    [SerializeField] private float fovSpeed = 4f;
    [SerializeField] private Camera playerCamera;
    private float targetFov;
    private float fov;

    private void Awake()
    {
        targetFov = playerCamera.fieldOfView;
        fov = targetFov;
    }

    void Update()
    {
        fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
        playerCamera.fieldOfView = fov;
    }

    public void SetCameraFov(float targetFov)
    {
        this.targetFov = targetFov;
    }

    public void ResetCameraFov()
    {
        this.targetFov = NORMAL_FOV;
    }
}
