using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    public float TurnSpeed = 15.0f;
    public float AimDuration = 0.3f;
    public Rig AimLayer;
    
    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
       
    }
    
    private void FixedUpdate()
    {
        float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera,0),TurnSpeed * Time.fixedDeltaTime );
    }

    private void LateUpdate()
    {
        if (AimLayer)
        {
            AimLayer.weight = 1.0f;
        }
    }
}
