using UnityEngine;
using UnityEditor.Animations;

public class ActiveWeapon : MonoBehaviour
{
    public UnityEngine.Animations.Rigging.Rig HandIK;
    public Transform CrossHairTarget;
    public Transform WeaponParent;
    public Transform WeaponRighGrip;
    public Transform WeaponLeftGrip;
    
    private RaycastWeapon _weapon;
    private Animator _animator;
    private AnimatorOverrideController _overrides;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _overrides =_animator.runtimeAnimatorController as AnimatorOverrideController;
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }
    
    private void Update()
    {
        if (_weapon)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _weapon.StartFiring();
            }
            if (_weapon.IsFiring)
            {
                _weapon.UpdateFiring(Time.deltaTime);
            }
            _weapon.UpdateBullet(Time.deltaTime);
            if (Input.GetButtonUp("Fire1"))
            {
                _weapon.StopFiring();
            }
        }
        else
        {
            HandIK.weight = 0.0f;
            _animator.SetLayerWeight(1,0.0f);
        }
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        if (_weapon)
        {
            Destroy(_weapon.gameObject);
        }
        _weapon = newWeapon;
        _weapon.RaycastDestination = CrossHairTarget;
        _weapon.transform.parent = WeaponParent;
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.identity;
        HandIK.weight = 1.0f;
        _animator.SetLayerWeight(1,1.0f);
        Invoke(nameof(SetAnimationDelay), 0.001f);
    }

    private void SetAnimationDelay()
    {
        _overrides["WeaponAnimationEmpty"] = _weapon.WeaponAnimation;
    }

    [ContextMenu("Save weapon pose")] private void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(WeaponParent.gameObject,false);
        recorder.BindComponentsOfType<Transform>(WeaponLeftGrip.gameObject,false);
        recorder.BindComponentsOfType<Transform>(WeaponRighGrip.gameObject,false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(_weapon.WeaponAnimation);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}
