using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private static float horizontalInput => Input.GetAxis("Horizontal");
    private static float verticalInput => Input.GetAxis("Vertical");
    private static float mouseHorizontal => Input.GetAxis("Mouse X");
    private static float mouseVertical => Input.GetAxis("Mouse Y");
    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float lookSensitivity = 180f;
    [SerializeField] private float rotateSensitivity = 360f;

    private float _cameraPitch;
    private CharacterController _cc;
    private Camera _playerCamera;
    private Transform _myTransform;
    
    void Start()
    {
        _myTransform = transform;
        _cc = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // --- Movement
        Vector3 moveVector = _myTransform.forward * verticalInput + _myTransform.right * horizontalInput;
        float movementSpeed = (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) * Time.deltaTime;
        _cc.Move(moveVector * movementSpeed);
        // --- Camera Pitch
        _cameraPitch += mouseVertical * lookSensitivity * Time.deltaTime * -1;
        _cameraPitch = ClampAngle(_cameraPitch, -85, 85);
        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0.0f, 0.0f);
        // --- Character Rotation
        float rotation = mouseHorizontal * rotateSensitivity * Time.deltaTime;
        _myTransform.Rotate(Vector3.up * rotation);
    }
    
    static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
