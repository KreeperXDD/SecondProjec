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
    public Cinemachine.CinemachineFreeLook PlayerCamera;
    public AmmoWidget AmmoWidget;
    
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
        if (weapon && !_isHolstered)
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
        weapon.Recoil.PlayerCamera = PlayerCamera;
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
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        _activeWeaponIndex = activateIndex;
    }

    private IEnumerator HolsterWeapon(int index)
    {
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
    }

    private IEnumerator ActivateWeapon(int index)
    {
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
    }
}
