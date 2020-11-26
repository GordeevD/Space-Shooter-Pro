using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    private bool _isEnemyLaser = false;
    private Transform _target = null;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser)
        {
            MoveDown(); 
        } else
        {
            if (_target == null)
            {
                MoveUp();
            }
            else
            {
                MoveToTarget();
            }
        }
    }

    void MoveToTarget()
    {
        rb.velocity = transform.up * 350f * Time.deltaTime;
        Vector3 targetVector = _target.position - transform.position;
        float rotatingIndex = Vector3.Cross(targetVector, transform.up).z;
        rb.angularVelocity = -1 * rotatingIndex * 2000f * Time.deltaTime;
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    public void AssignTarget(Transform target)
    {
        _target = target;
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }
    }

    void OnBecameInvisible()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(this.gameObject);
    }
}
