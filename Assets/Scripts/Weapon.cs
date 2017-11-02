﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private bool canShoot = true;

    private float FireRate;
    private float ProjectileSpeed;
    public Transform Muzzle;
    public Transform ProjectileParent;

    public Projectile MyProjectile;
    public WeaponStats MyWeaponStats;

    private void Start()
    {
        FireRate = MyWeaponStats.FireRate;
        ProjectileSpeed = MyWeaponStats.ProjectileSpeed;
    }

    public void Aim(Vector3 target)
    {
        Vector3 diff = target - Muzzle.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        Muzzle.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    public bool Shoot()
    {
        if (canShoot)
        {
            StartCoroutine(ShootingCooldown());

            Projectile newProjectile = Instantiate(MyProjectile, Muzzle.position, Muzzle.rotation);
            newProjectile.SetSpeed(ProjectileSpeed);
            newProjectile.transform.SetParent(ProjectileParent);

            return true;
        }
        return false;
    }

    private IEnumerator ShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(FireRate);
        canShoot = true;
    }
}