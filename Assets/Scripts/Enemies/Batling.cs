using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batling : MonoBehaviour
{
    public float attackCooldown;
    public Transform bulletPoint;
    public BulletSpawner[] bulletSpawners;

    private float cooldown;
    private Transform player;
    private SpriteRenderer sprite;
    private Color localColor;

    private Enemy enemy;

    private void Start()
    {
        cooldown = Random.Range(0f, attackCooldown);
        player = GameObject.FindGameObjectWithTag("Player").transform;

        enemy = GetComponent<Enemy>();
        sprite = GetComponent<SpriteRenderer>();
        localColor = sprite.color;
    }

    void Update()
    {
        if (enemy.isFreeze) return;

        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;

            if (cooldown < 1f)
            {
                if (!LeanTween.isTweening(gameObject))
                {
                    gameObject.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).setEaseInSine().setOnComplete(() =>
                    {
                        gameObject.LeanScale(Vector3.one, 0.1f).setEaseOutSine();
                        gameObject.LeanColor(Color.white, 1f).setEaseOutSine().setOnUpdate((x) =>
                        {
                            sprite.color = x;
                        });

                    });

                    gameObject.LeanColor(Color.red, 1f).setEaseInSine().setOnUpdate((x) =>
                    {
                        sprite.color = x;
                    });
                }
            }
        }
        else
        {
            Vector2 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            bulletPoint.rotation = rotation;

            foreach (BulletSpawner bulletSpawner in bulletSpawners)
            {
                bulletSpawner.Fire();
            }

            cooldown = attackCooldown;
            sprite.color = localColor;
        }
    }
}
