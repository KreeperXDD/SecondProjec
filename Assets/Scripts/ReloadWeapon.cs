using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator RigController;
    public WeaponAnimationEvents AnimationEvents;
    public ActiveWeapon ActiveWeapon;
    public Transform LeftHand;
    public AmmoWidget AmmoWidget;
    public bool IsReloading;

    private GameObject _magazineHand;
    private void Start()
    {
        AnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    private void Update()
    {
        RaycastWeapon weapon = ActiveWeapon.GetActiveWeapon();
        if (weapon)
        {
            if (Input.GetKeyDown(KeyCode.R) || weapon.AmmoCount <= 0)
            {
                IsReloading = true;
                RigController.SetTrigger("reloadWeapon");
            }
            if (weapon.IsFiring)
            {
                AmmoWidget.Refresh(weapon.AmmoCount);
            }
        }
    }

    private void OnAnimationEvent(string eventName)
    {
        Debug.Log(eventName);
        switch (eventName)
        {
           case "detachMagazine":
               DetachMagazine();
               break;
           case "dropMagazine":
               DropMagazine();
               break;
           case "refillMagazine":
               RefillMagazine();
               break;
           case "attachMagazine":
               AttachMagazine();
               break;
        }
    }

    private void DetachMagazine()
    {
        RaycastWeapon weapon = ActiveWeapon.GetActiveWeapon();
        _magazineHand = Instantiate(weapon.Magazine, LeftHand, true);
        weapon.Magazine.SetActive(false);
    }
    private void DropMagazine()
    {
        GameObject droppedMagazine = Instantiate(_magazineHand, _magazineHand.transform.position,
            _magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        _magazineHand.SetActive(false);
    }
    private void RefillMagazine()
    {
        _magazineHand.SetActive(true);
    }
    private void AttachMagazine()
    {
        RaycastWeapon weapon = ActiveWeapon.GetActiveWeapon();
        weapon.Magazine.SetActive(true);
        Destroy(_magazineHand);
        weapon.AmmoCount = weapon.ClipSize;
        RigController.ResetTrigger("reloadWeapon");
        AmmoWidget.Refresh(weapon.AmmoCount);
        IsReloading = false;
    }
}
