using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Rope : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private int quality;
    [SerializeField] private float damper;
    [SerializeField] private float strength;
    [SerializeField] private float velocity;
    [SerializeField] private float waveCount;
    [SerializeField] private float waveHeight;
    [SerializeField] private AnimationCurve affectCurve;

    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform gunpoint;
    [SerializeField] private FPSController fpsController;
    
    private Vector3 currentGHookPosition;
    private Spring spring;

    private void LateUpdate()
    {
        DrawRope();
    }

    private void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }

    public void DrawRope()
    {
        if (fpsController.CurrentState != State.GHookFlying && fpsController.CurrentState != State.GHookThrown) 
        {
            currentGHookPosition = fpsController.GunPoint.position;
            spring.Reset();
            if (lineRenderer.positionCount > 0)
            {
                lineRenderer.positionCount = 0;
            }
            return;
        }
        if (lineRenderer.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lineRenderer.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = fpsController.GHookPosition;
        Vector3 gunPointPosition = fpsController.GunPoint.position;

        Vector3 up = Quaternion.LookRotation((grapplePoint - gunPointPosition).normalized) * Vector3.up;

        currentGHookPosition = Vector3.Lerp(currentGHookPosition, grapplePoint, Time.deltaTime * 12f);

        for (int i = 0; i <quality +1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            lineRenderer.SetPosition(i, Vector3.Lerp(gunPointPosition, currentGHookPosition, delta) + offset);
        }
    }
}
