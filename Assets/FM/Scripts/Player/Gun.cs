using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Parameters")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float spread;
    [SerializeField] private float range;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private int bulletsPerTap;
    [SerializeField] private bool allowButtonHold;


    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CameraRecoilShake cameraShake;
    [SerializeField] private GunRecoil gunRecoil;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private RaycastHit hit;
    [SerializeField] private LayerMask shootableLayers;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ObjectPoolSO hotTrailObjectPool;
    [SerializeField] private ObjectPoolSO bulletHoleObjectPool;
    [SerializeField] private GameObject bulletDebree;

    private bool shooting;
    private CooldownTimer cooldownTimer;

    private void Awake()
    {
        cooldownTimer = new CooldownTimer();
    }

    private void Update()
    {
        cooldownTimer.UpdateCooldown(Time.deltaTime);
    }

    public void HandleGunInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (cooldownTimer.IsReady && shooting)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        for (int i = 0; i < bulletsPerTap; i++)
        {
            float z = UnityEngine.Random.Range(-spread, spread);
            float y = UnityEngine.Random.Range(-spread, spread);
            Vector3 direction = (playerCamera.transform.forward + new Vector3(0, y, z)).normalized;
            TrailRenderer trail;
            if (Physics.Raycast(playerCamera.transform.position, direction, out hit, range, shootableLayers))
            {
                if (hotTrailObjectPool.GetObjectAtPosition(gunPoint.position).TryGetComponent<TrailRenderer>(out trail))
                {
                    StartCoroutine(SpawnTrail(trail, hit, direction, true));
                }
            }
            else
            {
                if (hotTrailObjectPool.GetObjectAtPosition(gunPoint.position).TryGetComponent<TrailRenderer>(out trail))
                {
                    StartCoroutine(SpawnTrail(trail, hit, direction, false));
                }
            }
        }

        muzzleFlash.Play();
        cooldownTimer.StartCooldown(timeBetweenShots);
        cameraShake.ApplyRecoil();
        gunRecoil.ApplyRecoil();
    }

    /*
     * Oh Unity lords, forgive me this filth! I only ask for penance, for I myself cannot forgive this atrocity.
     */
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, Vector3 direction, bool madeImpact)
    {
        Vector3 startPosition = gunPoint.transform.position;
        float distance = Vector3.Distance(gunPoint.transform.position, hit.point);
        float remainingDistance = distance;
        float time = 0f;

        while (remainingDistance > 0f && time < 1f)
        {
            if (madeImpact)
            {
                trail.transform.position = Vector3.Lerp(startPosition, hit.point, 1 - (remainingDistance / distance));
                remainingDistance -= bulletSpeed * Time.deltaTime;
            }
            else
            {
                trail.transform.position = Vector3.Lerp(startPosition, direction * range, time);
                time += Time.deltaTime / trail.time;
            }
            yield return null;
        }

        if (madeImpact)
        {
            trail.transform.position = hit.point;
            Instantiate(bulletDebree, hit.point, Quaternion.LookRotation(-direction));
            GameObject bulletHoleGO = bulletHoleObjectPool.GetObject();
            bulletHoleGO.transform.position = hit.point;
            bulletHoleGO.transform.rotation = Quaternion.LookRotation(hit.normal);
            CheckForTarget(hit);
        }
        else
        {
            trail.transform.position = direction * range;
        }
    }

    private void CheckForTarget(RaycastHit hit)
    {
        ShootingTarget target;
        if (hit.collider.TryGetComponent<ShootingTarget>(out target))
        {
            target.OnTargetShot();
        }
    }
}
