using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;

    // ID for powerups
    // 0 = Triple Shot
    // 1 = Speed 
    // 2 = Shields
    // 3 = Ammo
    // 4 = Life
    // 5 = Projectile
    // 6 = Negative powerup
    //
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _audioClip;
    private SpawnManager _spawnManager;
    private GameObject _player;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private const float _rotateSpeed = 50f;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        _player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
        }
       
        if (_powerupID == 5)
        {
            transform.Rotate(Vector3.back * _rotateSpeed * Time.deltaTime);
        }

        if (transform.position.y < -4.5f)
        {
            OnPowerupDissolve();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);

            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoReboot();
                        break;
                    case 4:
                        player.Heal();
                        break;
                    case 5:
                        player.SecondaryFire();
                        break;
                    case 6:
                        player.NegativePowerup();
                        break;
                    default:
                        Debug.Log("Default Value " + _powerupID);
                        break;
                }
            }

            OnPowerupDissolve();
        }
        else if (other.CompareTag("Laser"))
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            OnPowerupDissolve(0.25f);
        }


        }
    private void OnPowerupDissolve(float delay = 0f)
    {
        Destroy(GetComponent<Collider2D>());
        _spawnManager.OnPowerupDissolve();
        Destroy(this.gameObject, delay);
    }
}
