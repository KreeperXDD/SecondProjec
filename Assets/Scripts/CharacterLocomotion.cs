using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private Animator _animator;
    private Vector2 _input;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        
        _animator.SetFloat("InputX", _input.x);
        _animator.SetFloat("InputY", _input.y);
    }
}
