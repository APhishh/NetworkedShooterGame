using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Weapon/GunData")]
public class GunData : ItemData
{

    public enum GunType
    {
        Automatic,
        Semi,
        Shotgun,
        BoltAction
    }

    public float rateOfFire;
    public float damage;
    public float inAccuracy;
    public int bulletsPerShot;
    public GunType gunType;
}
