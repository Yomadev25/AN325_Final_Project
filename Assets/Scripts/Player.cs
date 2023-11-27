using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField]
    private int _maxHp;
    [SerializeField]
    private int _hp;

    public float damage;

    [Header("Passive Rate")]
    [Range(0f, 1f)]
    public float criticalRate;
    [Range(0f, 1f)]
    public float boomRate;
    [Range(0f, 1f)]
    public float flameRate;
    [Range(0f, 1f)]
    public float iceRate;

    [Header("Move Setting")]
    [SerializeField]
    private Vector3 _offset;
    [SerializeField]
    private float _speed;

    private Camera cam;

    void Start()
    {
        _hp = _maxHp;
        cam = Camera.main;

        GameManager.instance.UpdateHPIcon(_maxHp, _hp);
        GameManager.instance.UpdatePassivePerk(criticalRate, flameRate, iceRate, boomRate);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Move(mousePos);
        }
    }

    void Move(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position + _offset, _speed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        Debug.Log("Take Damage");
        _hp--;

        GameManager.instance.UpdateHPIcon(_maxHp, _hp);
        if (_hp <= 0)
        {
            GameManager.instance.Gameover();
        }
    }

    public void Heal()
    {
        _hp++;
        if (_hp >= _maxHp) _hp = _maxHp;

        GameManager.instance.UpdateHPIcon(_maxHp, _hp);
    }

    public void AddHealth()
    {
        _maxHp++;
        if (_maxHp >= 10) _maxHp = 10;

        GameManager.instance.UpdateHPIcon(_maxHp, _hp);
    }
}
