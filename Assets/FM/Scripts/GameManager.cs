using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private List<ShootingTarget> targets = new List<ShootingTarget>();

    //TODO: Fix this so it everything is in one serializable list. Probably need OdinInspector or write custom.

    [SerializeField] private List<Transform> objectPoolParentList = new List<Transform>();
    [SerializeField] private List<ObjectPoolSO> objectPoolSOList = new List<ObjectPoolSO>();
    [SerializeField] private GameObject playerGO;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject winGameMenu;
    [SerializeField] private GameObject loseGameMenu;
    [SerializeField] private GameObject crossHair;

    private FPSController fpsPlayerController;
    private int targetsCleared = 0;
    private void OnEnable()
    {
        foreach (ShootingTarget target in targets)
        {
            target.OnTargetGreenEvent += TargetCleared;
        }
    }

    private void OnDisable()
    {
        foreach (ShootingTarget target in targets)
        {
            target.OnTargetGreenEvent -= TargetCleared;
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        for (int i = 0; i < objectPoolSOList.Count; i++)
        {
            if (objectPoolSOList[i] != null && objectPoolParentList[i])
            {
                objectPoolSOList[i].InitializePool(objectPoolParentList[i]);
            }
        }
    }

    private void Start()
    {
        fpsPlayerController = Instantiate(playerGO, spawnPoint.position, Quaternion.identity).GetComponent<FPSController>();
        BlockPlayerInput();
    }

    public void AllowPlayerInput()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crossHair.SetActive(true);
        if (fpsPlayerController)
        {
            fpsPlayerController.AllowInput(true);
        }
    }

    public void BlockPlayerInput()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crossHair.SetActive(false);
        if (fpsPlayerController)
        {
            fpsPlayerController.AllowInput(false);
        }
    }

    public void WinGame()
    {
        BlockPlayerInput();
        winGameMenu.SetActive(true);
    }

    public void LoseGame()
    {
        BlockPlayerInput();
        loseGameMenu.SetActive(true);
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void TargetCleared()
    {
        targetsCleared++;
        if (targetsCleared >= targets.Count)
        {
            WinGame();
        }
    }
}

