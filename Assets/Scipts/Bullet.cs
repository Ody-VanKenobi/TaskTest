using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifetime = 1f;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    
    private void Start()
    {
        Initialize();
        StartCoroutine(DestroyBullet());
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = transform.right * _speed;
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);    
    }

    private void Initialize()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 direction)
    {
        _rigidbody2D.velocity = direction.normalized * _speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<Health>();
        if (health != null && health.photonView.IsMine)
        {
            health.photonView.RPC(nameof(Health.Damage), RpcTarget.AllBuffered);
            Destroy(gameObject);
        }
    }
}
