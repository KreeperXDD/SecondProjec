using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public Cinemachine.CinemachineFreeLook PlayerCamera;
    [HideInInspector] public Cinemachine.CinemachineImpulseSource CameraShake;
    [HideInInspector] public Animator RigController;
    
    public Vector2[] RecoilPattern;
    public float Duration;
    
    private float _time;
    private int _index;
    private float _verticalRecoil;
    private float _horizontalRecoil;
    
    private void Awake()
    {
        CameraShake = GetComponent<CinemachineImpulseSource>();
    }

    public void Reset()
    {
        _index = 0;
    }

    private int NextIndex(int index)
    {
        return (index + 1) % RecoilPattern.Length;
    }

    public void GenerateRecoil(string weaponName)
    {
        _time = Duration;
        
        CameraShake.GenerateImpulse(Camera.main.transform.forward);

        _horizontalRecoil = RecoilPattern[_index].x;
        _verticalRecoil = RecoilPattern[_index].y;
        _index = NextIndex(_index);
        
        RigController.Play("WeaponRecoil" + weaponName,1,0.0f);
    }
    private void Update()
    {
        if (_time>0)
        {
            PlayerCamera.m_YAxis.Value -= ((_verticalRecoil/1000) * Time.deltaTime) / Duration;
            PlayerCamera.m_XAxis.Value -= ((_horizontalRecoil/10) * Time.deltaTime) / Duration;
            _time -= Time.deltaTime;
        }
    }
}
