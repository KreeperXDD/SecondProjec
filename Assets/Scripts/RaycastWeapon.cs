using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    private class  Bullet
    {
        public Vector3 InitialPosition;
        public Vector3 InitialVelocity;
        public TrailRenderer Tracer;
        public float Time;
    }
    
    public ParticleSystem[] MuzzleFlash;
    public ParticleSystem HitEffect;
    public TrailRenderer TracerEffect;
    public AnimationClip WeaponAnimation;
    public Transform RaycastOrigin;
    public Transform RaycastDestination;

    public bool IsFiring = false;
    public int FireRate = 25;
    public float BulletSpeed = 1000.0f;
    public float BulletDrop = 0.0f;
    
    private Ray _ray;
    private RaycastHit _hitInfo;
    private List<Bullet> _bullets = new List<Bullet>();
    private float _accumulatedTime;
    private float _maxLifeTime = 3.0f;

    private Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * BulletDrop;
        return bullet.InitialPosition + bullet.Time * bullet.InitialVelocity +
               bullet.Time * bullet.Time * 0.5f * gravity;
    }

    private Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.InitialPosition = position;
        bullet.InitialVelocity = velocity;
        bullet.Time = 0.0f;
        bullet.Tracer = Instantiate(TracerEffect, position, quaternion.identity);
        bullet.Tracer.AddPosition(position);
        return bullet;
    }
    
    public void StartFiring()
    {
        IsFiring = true;
        _accumulatedTime = 0.0f;
        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        _accumulatedTime += deltaTime;
        float fireInterval = 1.0f / FireRate;
        while (_accumulatedTime >= 0.0f)
        {
            FireBullet();
            _accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullet(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float deltaTime)
    {
        _bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.Time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullets()
    {
        _bullets.RemoveAll(bullet => bullet.Time >= _maxLifeTime);
    }

    private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        _ray.origin = start;
        _ray.direction = direction;
        if (Physics.Raycast(_ray, out _hitInfo,distance))
        {
            HitEffect.transform.position = _hitInfo.point;
            HitEffect.transform.forward = _hitInfo.normal;
            HitEffect.Emit(1);
        
            bullet.Tracer.transform.position = _hitInfo.point;
            bullet.Time = _maxLifeTime;
        }
        else
        {
            bullet.Tracer.transform.position = end;
        }
    }

    private void FireBullet()
    {
        foreach (var particle in MuzzleFlash)
        {
            particle.Emit(1);
        }

        Vector3 velocity = (RaycastDestination.position - RaycastOrigin.position).normalized * BulletSpeed;
        var bullet = CreateBullet(RaycastOrigin.position, velocity);
        _bullets.Add(bullet);
    }

    public void StopFiring()
    {
        IsFiring = false; 
    }
}
