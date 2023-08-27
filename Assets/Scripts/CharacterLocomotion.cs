using System;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public float JumpHeight;
    public float Gravity;
    public float StepDown;
    public float AirControl;
    public float JumpDump;
    
    private Animator _animator;
    private Vector2 _input;
    private Vector3 _rootMotion;
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isJumping;
    

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        
        _animator.SetFloat("InputX", _input.x);
        _animator.SetFloat("InputY", _input.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void OnAnimatorMove()
    {
        _rootMotion += _animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        if (_isJumping)
        {
            _velocity.y -= Gravity * Time.fixedDeltaTime;
            Vector3 displacement = _velocity * Time.fixedDeltaTime;
            displacement += CalculateAirControl();
            _controller.Move(displacement);
            _isJumping = !_controller.isGrounded;
            _rootMotion = Vector3.zero;
        }
        else
        {
            _controller.Move(_rootMotion + Vector3.down * StepDown);
            _rootMotion = Vector3.zero;

            if (!_controller.isGrounded)
            {
                _isJumping = true;
                _velocity = _animator.velocity*JumpDump;
                _velocity.y = 0;
            }
        }
    }

    private Vector3 CalculateAirControl()
    {
        return ((transform.forward * _input.y) + (transform.right * _input.x)) * (AirControl / 100);
    }

    private void Jump()
    {
        if (!_isJumping)
        {
            _isJumping = true;
            _velocity = _animator.velocity * JumpDump;
            _velocity.y = (float)Math.Sqrt(2 * Gravity *JumpHeight);
        }
    }
}
