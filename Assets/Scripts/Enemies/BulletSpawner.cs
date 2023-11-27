using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public enum SpawnType
    {
        Straight,
        Spin
    }

    public GameObject bullet;
    public float bulletLife;
    public float speed;

    public SpawnType spawnType;

    private GameObject spawnedBullet;

    private void Update()
    {
        if (spawnType == SpawnType.Spin)
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 1f);
        }
    }

    public void Fire()
    {
        if (bullet)
        {
            spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            Bullet bul = spawnedBullet.GetComponent<Bullet>();
            bul.speed = speed;
            bul.bulletLife = bulletLife;
            spawnedBullet.transform.rotation = transform.rotation;
        }
    }
}
