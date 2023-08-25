using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _hitInfo;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        var transform1 = _mainCamera.transform;
        _ray.origin = transform1.position;
        _ray.direction = transform1.forward;
        Physics.Raycast(_ray, out _hitInfo);
        transform.position = _hitInfo.point;
    }
}
