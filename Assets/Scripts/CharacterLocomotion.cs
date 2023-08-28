using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterLocomotion : MonoBehaviour
{
    public Animator RigController;
    public float JumpHeight;
    public float Gravity;
    public float StepDown;
    public float AirControl;
    public float JumpDump;
    public float GroundSpeed;
    public float PushPower;
    
    private CharacterController _controller;
    private CharacterAiming _characterAiming;
    private Animator _animator;
    private ActiveWeapon _activeWeapon;
    private ReloadWeapon _reloadWeapon;
    private Vector2 _input;
    private Vector3 _rootMotion;
    private Vector3 _velocity;
    private bool _isJumping;
    private int _isSprintingParameter = Animator.StringToHash("isSprinting");
    

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _activeWeapon = GetComponent<ActiveWeapon>();
        _reloadWeapon = GetComponent<ReloadWeapon>();
        _characterAiming = GetComponent<CharacterAiming>();
    }
    
    private void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        
        _animator.SetFloat("InputX", _input.x);
        _animator.SetFloat("InputY", _input.y);
        
        UpdateIsSprinting();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    private bool IsSprinting()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isFiring = _activeWeapon.IsFiringActive();
        bool isReloading = _reloadWeapon.IsReloading;
        bool isChangingWeapon = _activeWeapon.IsChangingWeapon;
        bool isAiming = _characterAiming.IsAiming;
        return isSprinting && !isFiring && !isReloading && !isChangingWeapon && !isAiming;
    }
    private void UpdateIsSprinting()
    {
        bool isSprinting = IsSprinting();
        _animator.SetBool(_isSprintingParameter,isSprinting);
        RigController.SetBool(_isSprintingParameter, isSprinting);
    }

    private void OnAnimatorMove()
    {
        _rootMotion += _animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        if (_isJumping)
        {
            UpdateInAir();
        }
        else
        {
            UpdateOnGround();
        }
    }

    private void UpdateOnGround()
    {

        Vector3 stepForwardAmount = _rootMotion * GroundSpeed;
        Vector3 stepDownAmount = Vector3.down * StepDown;
        
        _controller.Move(stepForwardAmount + stepDownAmount);
        _rootMotion = Vector3.zero;

        UpdateIsSprinting();
        
        if (!_controller.isGrounded)
        {
            SetInAir(0);
        }
    }


    private void UpdateInAir()
    {
        _velocity.y -= Gravity * Time.fixedDeltaTime;
        Vector3 displacement = _velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        _controller.Move(displacement);
        _isJumping = !_controller.isGrounded;
        _rootMotion = Vector3.zero;
        _animator.SetBool("isJumping", _isJumping);
    }

    private Vector3 CalculateAirControl()
    {
        return ((transform.forward * _input.y) + (transform.right * _input.x)) * (AirControl / 100);
    }

    private void Jump()
    {
        if (!_isJumping)
        {
            float jumpVelocity = (float)Math.Sqrt(2 * Gravity *JumpHeight);
            SetInAir(jumpVelocity);
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        _isJumping = true;
        _velocity = _animator.velocity * JumpDump * GroundSpeed;
        _velocity.y = jumpVelocity;
        _animator.SetBool("isJumping", true);
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;
        if (hit.moveDirection.y < -0.3f)
            return;
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * PushPower;
    }
    
    
}
