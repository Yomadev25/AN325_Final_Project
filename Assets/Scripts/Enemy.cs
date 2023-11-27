using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Property")]
    [SerializeField]
    private float _maxHp;
    [SerializeField]
    private float _hp;

    [Header("References")]
    [SerializeField]
    private Rigidbody2D _rb;

    [Header("Effects")]
    [SerializeField]
    private GameObject _slashEffect;
    [SerializeField]
    private GameObject _criticalEffect;
    [SerializeField]
    private GameObject _bombEffect;
    [SerializeField]
    private GameObject _fireEffect;
    [SerializeField]
    private GameObject _burnEffect;
    [SerializeField]
    private GameObject _iceEffect;
    [SerializeField]
    private GameObject _freezeEffect;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onTakeDamage;
    [SerializeField]
    private UnityEvent onDie;

    public bool isFreeze;

    //Coroutine
    private Coroutine flameCoroutine;
    private Coroutine iceCoroutine;

    void Start()
    {
        _hp = _maxHp;
    }

    private void TakeDamage(float damage, Vector3 dealerPos = default, bool isCritical = false, bool isBomb = false, bool isFlame = false, bool isIce = false)
    {
        if (isCritical)
        {
            damage += damage * 0.3f;
            Instantiate(_criticalEffect, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_slashEffect, transform.position, Quaternion.identity);
        }

        if (isBomb)
        {
            damage += 10;
            Instantiate(_bombEffect, transform.position, Quaternion.identity);

            var bombRadius = Physics2D.OverlapCircleAll(transform.position, 1f);
            if (bombRadius.Length > 0)
            {
                foreach (Collider2D collision in bombRadius)
                {
                    if (collision.CompareTag("Enemy"))
                    {
                        Enemy enemy = collision.GetComponent<Enemy>();
                        enemy.TakeDamage(5);
                    }
                }
            }
        }
        if (isFlame)
        {
            if (flameCoroutine == null)
            {
                flameCoroutine = StartCoroutine(FlameCoroutine());
            }
            else
            {
                StopCoroutine(flameCoroutine);
                flameCoroutine = StartCoroutine(FlameCoroutine());
            }
            
        }
        if (isIce)
        {
            if (iceCoroutine == null)
            {
                iceCoroutine = StartCoroutine(IceCoroutine());
            }
            else
            {
                StopCoroutine(iceCoroutine);
                iceCoroutine = StartCoroutine(IceCoroutine());
            }
        }
        
        _hp -= damage;
        GameManager.instance.AddTotalDamage(damage);

        if (_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.AddProgress(_maxHp * 0.1f);

        onDie?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            Vector3 playerPos = player.transform.position;

            bool isCritical = (Random.Range(0f, 1f) < player.criticalRate);
            bool isBomb = (Random.Range(0f, 1f) < player.boomRate);
            bool isFlame = (Random.Range(0f, 1f) < player.flameRate);
            bool isIce = (Random.Range(0f, 1f) < player.iceRate);

            TakeDamage(player.damage, playerPos, isCritical, isBomb, isFlame, isIce);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.SpawnCheck();
    }

    #region STATUS

    IEnumerator FlameCoroutine()
    {
        GameObject fire = Instantiate(_fireEffect, transform);
        Destroy(fire, 5f);

        for (int i = 0; i < 5; i++)
        {
            TakeDamage(5f);
            yield return new WaitForSeconds(1f);
        }      
    }

    IEnumerator IceCoroutine()
    {
        Instantiate(_iceEffect, transform);
        GameObject fire = Instantiate(_freezeEffect, transform);
        Destroy(fire, 5f);
        isFreeze = true;

        yield return new WaitForSeconds(5f);

        isFreeze = false;
    }

    #endregion
}
