using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBomb : MonoBehaviour
{
    public BulletSpawner[] bulletSpawners;

    public void Explode()
    {
        foreach (BulletSpawner bulletSpawner in bulletSpawners)
        {
            bulletSpawner.Fire();
        }
    }
}
