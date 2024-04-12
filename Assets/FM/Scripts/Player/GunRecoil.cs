using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Reference Points:")]
    [SerializeField] private Transform gunTransform;

    [SerializeField] private float positionalRecoilSpeed = 8f;
    [SerializeField] private float rotationalRecoilSpeed = 8f;

    [SerializeField] private float positionalReturnSpeed = 18f;
    [SerializeField] private float rotationalReturnSpeed = 38f;

    [SerializeField] private Vector3 recoilRotation = new Vector3(10f, 5f, 7f);
    [SerializeField] private Vector3 recoilKickBack = new Vector3(0.015f, 0f, -0.2f);

    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 targetRotation;

    private void FixedUpdate()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

        gunTransform.localPosition = Vector3.Slerp(gunTransform.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        targetRotation = Vector3.Slerp(targetRotation, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        gunTransform.localRotation = Quaternion.Euler(targetRotation);
    }


    public void ApplyRecoil()
    {
        rotationalRecoil += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        positionalRecoil += new Vector3(Random.Range(-recoilKickBack.x, recoilKickBack.x), Random.Range(-recoilKickBack.y, recoilKickBack.y), recoilKickBack.z);
    }
}
