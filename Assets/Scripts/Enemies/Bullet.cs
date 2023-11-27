using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        Normal,
        Homing
    }

    public float bulletLife;
    public float speed;
    public BulletType type = BulletType.Normal;

    private Vector2 spawnPoint;
    private float timer;
    private Transform player;

    public UnityEvent onDestroyed;

    private void Start()
    {      
        spawnPoint = new Vector2(transform.position.x, transform.position.y);

        if (type == BulletType.Homing)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (timer > bulletLife)
        {
            onDestroyed?.Invoke();
            Destroy(this.gameObject);
        }
        timer += Time.deltaTime;
        
        if (type == BulletType.Normal)
        {
            transform.position = Movement(timer);
        }
        else if (type == BulletType.Homing)
        {
            FollowHoming();
        }
    }

    private Vector2 Movement(float timer)
    {
        float x = timer * speed * transform.right.x;
        float y = timer * speed * transform.right.y;

        return new Vector2(x + spawnPoint.x, y + spawnPoint.y);
    }

    private void FollowHoming()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);

            transform.position = (Vector2)transform.position + (new Vector2(direction.x, direction.y)) * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            player.TakeDamage();

            Destroy(this.gameObject);
        }
    }
}
