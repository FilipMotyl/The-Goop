using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        FPSController controller = other.GetComponent<FPSController>();
        if (controller)
        {
            Destroy(controller);
        }
        gameManager.LoseGame();
    }
}
