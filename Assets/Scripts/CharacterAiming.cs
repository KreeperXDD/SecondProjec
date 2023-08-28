using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class CharacterAiming : MonoBehaviour
{
    public float TurnSpeed = 15.0f;
    public float AimDuration = 0.3f;
    public Cinemachine.AxisState XAxis;
    public Cinemachine.AxisState YAxis;
    public Transform CameraLookAt;
    [FormerlySerializedAs("isAiming")] public bool IsAiming;

    private Camera _mainCamera;
    private Animator _animator;
    private ActiveWeapon _activeWeapon;
    private int _isAimingParameter = Animator.StringToHash("isAiming");

    private void Start()
    {
        _mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _animator = GetComponent<Animator>();
        _activeWeapon = GetComponent<ActiveWeapon>();
    }

    private void Update()
    {
        IsAiming = Input.GetMouseButton(1);
        _animator.SetBool(_isAimingParameter, IsAiming);

        var weapon = _activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            weapon.Recoil.RecoilModifier = IsAiming ? 0.4f : 1.0f;
        }
    }

    private void FixedUpdate()
    {
        XAxis.Update(Time.fixedDeltaTime);
        YAxis.Update(Time.fixedDeltaTime);
        CameraLookAt.eulerAngles = new Vector3(YAxis.Value, XAxis.Value, 0);
        float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera,0),TurnSpeed * Time.fixedDeltaTime );
    }
}
