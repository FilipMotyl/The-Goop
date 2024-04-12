using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilShake : MonoBehaviour
{
    [Header("Recoil Settings:")]
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float returnSpeed = 25f;

    [Header("Hipfire:")]
    [SerializeField] private Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);

    private Vector3 currentRecoilRotation;
    private Vector3 targetRecoilRotation;

    private void FixedUpdate()
    {
        currentRecoilRotation = Vector3.Lerp(currentRecoilRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        targetRecoilRotation = Vector3.Slerp(targetRecoilRotation, currentRecoilRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(targetRecoilRotation);
    }

    public void ApplyRecoil()
    {
        currentRecoilRotation += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
    }
}
