using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatManager : NetworkBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    private PlayerInventory playerInventory;
    private bool shootingOnCooldown;


    void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {

        if (!IsOwner) return;
        if (CheckIfWeapon())
        {
            HandleWeapons();
        }
    }

    private ItemData GetEquipped()
    {
        return playerInventory.getEquipped();
    }

    private bool CheckIfWeapon()
    {
        if (GetEquipped() == null) return false;
        return (GetEquipped().itemType == ItemData.ItemType.Melee || GetEquipped().itemType == ItemData.ItemType.Gun);
    }

    private void HandleWeapons()
    {
        switch (GetEquipped().itemType)
        {
            case ItemData.ItemType.Gun:
                HandleGun();
                break;
        }
    }

    #region  Gun Handling
    private void HandleGun()
    {
        switch (((GunData)GetEquipped()).gunType)
        {
            case GunData.GunType.Automatic:
                HandleAutomaticGun();
                break;
            case GunData.GunType.Shotgun:
                HandleShotgun();
                break;
        }
    }

    private void HandleAutomaticGun()
    {
        if (Input.GetMouseButton(0) && !shootingOnCooldown)
        {
            shootingOnCooldown = true;
            StartCoroutine(ShootCooldown());
            setupBullet();
        }
    }

    private void HandleShotgun()
    {
        if (Input.GetMouseButtonDown(0) && !shootingOnCooldown)
        {
            shootingOnCooldown = true;
            StartCoroutine(ShootCooldown());
            FireShotgun();
        }
    }

    private void FireShotgun()
    {
        for (int i = 0; i < ((GunData)GetEquipped()).bulletsPerShot; i++)
        {
            setupBullet();
        }
    }
    #endregion
    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(((GunData)GetEquipped()).rateOfFire);
        shootingOnCooldown = false;
    }

    private void setupBullet()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position);
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + AddRecoil();

        
        SpawnLocalBullet(angle, transform.position);
      
        
        //SpawnBulletServerRpc(angle, transform.position); FUCK THIS SHIT

    }

    private float AddRecoil()
    {
        float inAccuracy = ((GunData)GetEquipped()).inAccuracy;
        return UnityEngine.Random.Range(-inAccuracy, inAccuracy);
    }

    private void SpawnLocalBullet(float angle, Vector2 spawnPos)
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle - 90));
        var bulletBehavior = bullet.GetComponent<BulletBehavior>();
        ulong shootedId = NetworkManager.Singleton.LocalClientId;

        bulletBehavior.Shoot(transform.position, shootedId);
        
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(float angle, Vector2 spawnPos) //UNUSED MAYBE WILL BE REUSED SOMEDAY
    {

        var serverBullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle - 90));
        var bulletBehavior = serverBullet.GetComponent<BulletBehavior>();
        var bulletNetworkObject = serverBullet.GetComponent<NetworkObject>();
        bulletNetworkObject.Spawn();
        ulong shootedId = NetworkManager.Singleton.LocalClientId;
        bulletBehavior.Shoot(transform.position, shootedId);
        //bulletBehavior.setVelocity(bullet.transform.up * bulletSpeed, true);

    }


}
