using System.Collections;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Serialization;

public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
       Primary = 0,
       Secondary = 1
    }

    public Transform CrossHairTarget;
    public Animator RigController;
    public Transform[] WeaponSlots;
    public CharacterAiming Aiming;
    public AmmoWidget AmmoWidget;
    public bool IsChangingWeapon = false;
    
    private RaycastWeapon[] _equippedWeapons = new RaycastWeapon[2];
    private int _activeWeaponIndex;
    private bool _isHolstered = false;

    private void Start()
    {
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    public bool IsFiringActive()
    {
        RaycastWeapon currentWeapon = GetActiveWeapon();
        if (!currentWeapon)
        {
            return false;
        }
        return currentWeapon.IsFiring;
    }

    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(_activeWeaponIndex);
    }
    
    private RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index > _equippedWeapons.Length)
        {
            return null;
        }
        return _equippedWeapons[index];
    }
    
    private void Update()
    {
        var weapon = GetWeapon(_activeWeaponIndex);
        bool notSprinting = RigController.GetCurrentAnimatorStateInfo(2).shortNameHash ==
                            Animator.StringToHash("notSprinting");
        if (weapon && !_isHolstered && notSprinting)
        {
            weapon.UpdateWeapon(Time.deltaTime);
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleActiveWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(WeaponSlot.Primary);
        }if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(WeaponSlot.Secondary);
        }
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.WeaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon)
        {
            Destroy(weapon.gameObject);
        }
        
        weapon = newWeapon;
        weapon.RaycastDestination = CrossHairTarget;
        weapon.Recoil.Aiming = Aiming;
        weapon.Recoil.RigController = RigController;
        weapon.transform.SetParent(WeaponSlots[weaponSlotIndex],false);
        _equippedWeapons[weaponSlotIndex] = weapon;
        SetActiveWeapon(newWeapon.WeaponSlot);
        AmmoWidget.Refresh(weapon.AmmoCount);
    }

    private void ToggleActiveWeapon()
    {
        bool isHolstered = RigController.GetBool("holsterWeapon");
        if (isHolstered)
        {
            StartCoroutine(ActivateWeapon(_activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(_activeWeaponIndex));
        }
    }

    private void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = _activeWeaponIndex;
        int activateIndex = (int)weaponSlot;
        
        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }
        
        StartCoroutine(SwitchWeapon(holsterIndex,activateIndex));
    }
    
    private IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        RigController.SetInteger("weaponIndex", activateIndex);
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        _activeWeaponIndex = activateIndex;
    }

    private IEnumerator HolsterWeapon(int index)
    {
        IsChangingWeapon = true;
        _isHolstered = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            RigController.SetBool("holsterWeapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (RigController.GetCurrentAnimatorStateInfo(0).normalizedTime<1.0f);
        }
        IsChangingWeapon = false;
    }

    private IEnumerator ActivateWeapon(int index)
    {
        IsChangingWeapon = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            RigController.SetBool("holsterWeapon", false);
            RigController.Play("equip" + weapon.WeaponName,0);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (RigController.GetCurrentAnimatorStateInfo(0).normalizedTime<1.0f);
            _isHolstered = false;
        }
        IsChangingWeapon = false;
    }
}
