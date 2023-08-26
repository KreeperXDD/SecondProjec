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
    public float VerticalRecoil;
    public float Duration;

    private float _time;

    private void Awake()
    {
        CameraShake = GetComponent<CinemachineImpulseSource>();
    }

    public void GenerateRecoil()
    {
        _time = Duration;
        
        CameraShake.GenerateImpulse(Camera.main.transform.forward);
    }
    private void Update()
    {
        if (_time>0)
        {
            PlayerCamera.m_YAxis.Value -= ((VerticalRecoil/1000) * Time.deltaTime) / Duration;
            _time -= Time.deltaTime;
        }
    }
}
