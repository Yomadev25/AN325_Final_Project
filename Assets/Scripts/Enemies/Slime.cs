using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public float biteCooldown;
    public float biteRadius;
    public GameObject[] childs;

    private float cooldown;
    private bool isBite;
    private SpriteRenderer sprite;
    private Color localColor;

    private Enemy enemy;
    private Vector3 localScale;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        sprite = GetComponent<SpriteRenderer>();
        localColor = sprite.color;
        localScale = transform.localScale;
        cooldown = Random.Range(0f, biteCooldown);
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
                    gameObject.LeanScale(localScale + new Vector3(0.1f, 0.1f, 0.1f), 1f).setEaseInSine().setOnComplete(() =>
                    {
                        gameObject.LeanScale(localScale, 0.1f).setEaseOutSine();
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
            if (!isBite)
            {
                isBite = true;
                StartCoroutine(BiteCoroutine());
            }
        }
    }

    IEnumerator BiteCoroutine()
    {
        Player player = FindObjectOfType<Player>();
        if (Vector3.Distance(transform.position, player.transform.position) < biteRadius)
        {
            player.TakeDamage();
        }

        yield return new WaitForSeconds(1f);
        isBite = false;

        cooldown = biteCooldown;
        sprite.color = localColor;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < childs.Length; i++)
        {
            if (!GameManager.instance.isPlay) return;

            var spawnPos = new Vector3();
            spawnPos.x = transform.position.x + Random.Range(0.5f, 1.5f);
            spawnPos.y = transform.position.y + Random.Range(0.5f, 1.5f);

            Instantiate(childs[i], spawnPos, Quaternion.identity);
        }
    }
}
