using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> cyllinderMeshList = new List<MeshRenderer>();
    [SerializeField] private List<ParticleSystem> confettiList = new List<ParticleSystem>();
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Collider targetCollider;

    private int greenMeshNum = 0;

    public delegate void TargetGreenEvent();

    public event TargetGreenEvent OnTargetGreenEvent;
    public void OnTargetShot()
    {
        if (cyllinderMeshList.Count > 0 && greenMeshNum < cyllinderMeshList.Count)
        {
            cyllinderMeshList[greenMeshNum].material = greenMaterial;
            greenMeshNum++;
        }

        if (greenMeshNum > 0 && greenMeshNum == cyllinderMeshList.Count)
        {
            foreach (ParticleSystem confetti in confettiList)
            {
                confetti.Play();
            }
            greenMeshNum++;
            OnTargetGreenEvent?.Invoke();
        }
    }
}
