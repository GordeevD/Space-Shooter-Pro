using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource is NULL");
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {

            _speed = 0;
            _anim.SetTrigger("OnEnemyDeath");

            if (_player != null)
            {
                _player.Damage();
            }
            _audioSource.Play();
            Destroy(this.gameObject, 2.4f);

        }
        if (other.CompareTag("Laser"))
        {
            _speed = 0;
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(other.gameObject);
            //add score 10
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _audioSource.Play();
            Destroy(this.gameObject, 2.4f);
        }
    }

}
