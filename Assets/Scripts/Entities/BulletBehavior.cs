using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D bulletRb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject visualBullet;

    private ulong bulletID;
    private Vector2 origin;
    private GameObject visualBulletInstance;
    private bool fired;
    private List<GameObject> alreadyHit = new List<GameObject>();

    void Start()
    {
        StartCoroutine(destroyBullet());
    }

    void Update()
    {
        if (fired)
        {
            RaycastHit2D ray = Physics2D.Raycast(origin, transform.up * bulletSpeed * Time.deltaTime);
            Debug.DrawRay(origin, transform.up * 1);
            origin += (Vector2)transform.up * bulletSpeed * Time.deltaTime;

            if (visualBulletInstance != null) visualBulletInstance.transform.position = origin;

            if (ray.collider != null && ray.collider.gameObject.tag == "Enemy" && !alreadyHit.Contains(ray.collider.gameObject))
            {

                alreadyHit.Add(ray.collider.gameObject);
                hitTarget(ray.collider.gameObject);
            }
        }
    }

    public void Shoot(Vector2 startPos, ulong clientID)
    {
        bulletID = clientID;
        origin = startPos;
        fired = true;


        Vector2 dir = transform.up;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle - 90);
        visualBulletInstance = Instantiate(visualBullet, transform.position, rot);
    }

    private IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(2f);
        if (visualBulletInstance != null) Destroy(visualBulletInstance);
        Destroy(gameObject);
    }

    private void hitTarget(GameObject obj)
    {
        
        if (obj.GetComponent<EnemyProperties>() == null) return;
        EnemyProperties enemyProperties = obj.GetComponent<EnemyProperties>();

        if (enemyProperties == null) Debug.LogError("Enemy Properties doesn't exist.");

        float ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId);

        enemyProperties.ReceiveHitServerRpc(obj.transform.position, ping, bulletID, 100);


    }
}
